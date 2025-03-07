using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class TarifaRepository : BaseRepository<Tarifas, int>, ITarifaRepository
    {
        private readonly ILogger<TarifaRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IValidator<Tarifas> _validator;

        public TarifaRepository(
            HRMSContext context,
            ILogger<TarifaRepository> logger,
            IConfiguration configuration,
            IValidator<Tarifas> validator
        ) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
        }

        public override async Task<List<Tarifas>> GetAllAsync()
        {
            return await _context.Tarifas.Where(t => t.Estado == true).ToListAsync();
        }

        public override async Task<Tarifas> GetEntityByIdAsync(int id)
        {
            if (id <=0) throw new ArgumentException("ID no válido", nameof(id));

            return await _context.Set<Tarifas>().FindAsync(id)
                   ?? throw new InvalidOperationException($"No se encontró la tarifa con ID: {id}");
        }

        public override async Task<OperationResult> SaveEntityAsync(Tarifas tarifas)
        {
            var validation = ValidarTarifa(tarifas);
            if (!validation.IsSuccess) return validation;
    
            if (await _context.Tarifas.AnyAsync(t => t.PrecioPorNoche == tarifas.PrecioPorNoche))
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ya existe una tarifa con el precio '{tarifas.PrecioPorNoche}'."
                };
            }
    
            return await ExecuteDatabaseOperationAsync(async () =>
            {
                _context.Tarifas.Add(tarifas);
                await _context.SaveChangesAsync();
            }, "Tarifa guardada exitosamente.", tarifas);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifas)
        {
            var validation = ValidarTarifa(tarifas);
            if (!validation.IsSuccess) return validation;

            var existingTarifa = await _context.Tarifas.FindAsync(tarifas.IdTarifa);
            if (existingTarifa == null)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"No se encontró la tarifa con ID: {tarifas.IdTarifa}."
                };
            }

            UpdateTarifa(existingTarifa, tarifas);
            return await ExecuteDatabaseOperationAsync(async () =>
            {
                await _context.SaveChangesAsync();
            }, "Tarifa actualizada correctamente.", existingTarifa);
        }

        public async Task<OperationResult> GetTarifasVigentesAsync(string fechaInput)
        {
            var fechaValidation = ValidateFechaFormat(fechaInput);
            if (!fechaValidation.IsSuccess) return fechaValidation;

            DateTime fecha = (DateTime)fechaValidation.Data;

            var tarifas = await _context.Tarifas
                .Where(t => t.Estado == true && t.FechaInicio <= fecha && t.FechaFin >= fecha)
                .ToListAsync();
            
            return tarifas.Any()
                ? new OperationResult { IsSuccess = true, Data = tarifas }
                : new OperationResult { IsSuccess = false, Message = $"No se encontraron tarifas vigentes para la fecha {fecha:yyyy-MM-dd}." };
        }

        public async Task<OperationResult> GetHabitacionByPrecioAsync(decimal precio)
        {
            if (precio <= 0)
                return new OperationResult { IsSuccess = false, Message = "El precio de la tarifa debe ser mayor a 0." };

            var fechaActual = DateTime.Now;
            var tarifas = await _context.Tarifas
                                        .Where(t => t.PrecioPorNoche == precio && t.Estado == true &&
                                                    t.FechaInicio <= fechaActual && t.FechaFin >= fechaActual)
                                        .ToListAsync();

            if (!tarifas.Any())
            {
                return new OperationResult { IsSuccess = false, Message = "No se encontraron tarifas vigentes con el precio indicado." };
            }

            var categoriaIds = tarifas.Select(t => t.IdCategoria).ToList();
            var habitaciones = await _context.Habitaciones
                                             .Where(h => h.Estado == true && h.IdCategoria.HasValue && categoriaIds.Contains(h.IdCategoria.Value))
                                             .ToListAsync();

            return habitaciones.Any()
                ? new OperationResult { IsSuccess = true, Data = habitaciones }
                : new OperationResult { IsSuccess = false, Message = "No se encontraron habitaciones con el precio de tarifa indicado." };
        }

        private OperationResult ValidarTarifa(Tarifas tarifa)
        {
            return _validator.Validate(tarifa);
        }

        private static void UpdateTarifa(Tarifas target, Tarifas source)
        {
            target.PrecioPorNoche = source.PrecioPorNoche;
            target.FechaInicio = source.FechaInicio;
            target.FechaFin = source.FechaFin;
            target.Estado = source.Estado;
        }

        private async Task<OperationResult> ExecuteDatabaseOperationAsync(Func<Task> operation, string successMessage, dynamic? data = null)
        {
            try
            {
                await operation();
                return new OperationResult { IsSuccess = true, Message = successMessage, Data = data };
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Error de concurrencia al actualizar la tarifa.");
                return new OperationResult { IsSuccess = false, Message = "La tarifa ya ha sido modificada por otro usuario. Intente nuevamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la operación.");
                return new OperationResult { IsSuccess = false, Message = $"Error en la operación: {ex.Message}" };
            }
        }

        private static OperationResult ValidateFechaFormat(string fechaInput)
        {
            if (string.IsNullOrWhiteSpace(fechaInput))
                return new OperationResult { IsSuccess = false, Message = "La fecha no puede estar vacía." };

            string[] formatosValidos = { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" };
            if (DateTime.TryParseExact(fechaInput, formatosValidos, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fecha))
            {
                return new OperationResult { IsSuccess = true, Data = fecha };
            }

            return new OperationResult { IsSuccess = false, Message = "El formato de la fecha es incorrecto. Usa formatos válidos: dd/MM/yyyy, yyyy-MM-dd, MM/dd/yyyy, dd-MM-yyyy." };
        }
    }
}
