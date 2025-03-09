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

        public override async Task<Tarifas?> GetEntityByIdAsync(int id)
        {
            if (id <= 0) return null;
    
            return await _context.Set<Tarifas>().FindAsync(id);
        }
        public override async Task<OperationResult> SaveEntityAsync(Tarifas tarifas)
        {
            var validation = ValidarTarifa(tarifas);
            if (!validation.IsSuccess) return validation;
            
            await _context.Tarifas.AddAsync(tarifas);
            await _context.SaveChangesAsync();
            return OperationResult.Success(tarifas, "Tarifa guardada correctamente.");
        }

        public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifas)
        {
            var validation = ValidarTarifa(tarifas);
            if (!validation.IsSuccess) return validation;

            var existingTarifa = await _context.Tarifas.FindAsync(tarifas.IdTarifa);
            if (existingTarifa == null)
            {
                return OperationResult.Failure("La tarifa no existe.");
            }

            UpdateTarifa(existingTarifa, tarifas);
            await _context.SaveChangesAsync();
            return OperationResult.Success(existingTarifa, "Tarifa actualizada correctamente.");

        }

        public async Task<OperationResult> GetTarifasVigentesAsync(string fechaInput)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(fechaInput))
                {
                    return OperationResult.Failure("La fecha no puede estar vacía.");
                }
        
                DateTime fecha;
                string[] formatosValidos = { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" };
        
                if (!DateTime.TryParseExact(fechaInput, formatosValidos, 
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out fecha))
                {
                    return OperationResult.Failure("Formato de fecha no válido.");
                }

                var tarifas = await _context.Tarifas
                    .Where(t => t.Estado == true && t.FechaInicio <= fecha && t.FechaFin >= fecha)
                    .ToListAsync();

                return tarifas.Any()
                    ? OperationResult.Success(tarifas, "Tarifas encontradas")
                    : OperationResult.Failure($"No se encontraron tarifas vigentes para la fecha {fecha:yyyy-MM-dd}.");
            });
        }

        public async Task<OperationResult> GetHabitacionByPrecioAsync(decimal precio)
        {
            if (precio <= 0)
            {
                return OperationResult.Failure("El precio debe ser mayor a 0.");
            }

            var fechaActual = DateTime.Now;
            var tarifas = await _context.Tarifas
                                        .Where(t => t.PrecioPorNoche == precio && t.Estado == true &&
                                                    t.FechaInicio <= fechaActual && t.FechaFin >= fechaActual)
                                        .ToListAsync();

            if (!tarifas.Any())
            {
                return OperationResult.Failure("No se encontraron tarifas vigentes con el precio indicado.");
            }

            var categoriaIds = tarifas.Select(t => t.IdCategoria).ToList();
            var habitaciones = await _context.Habitaciones
                                             .Where(h => h.Estado == true && h.IdCategoria.HasValue && categoriaIds.Contains(h.IdCategoria.Value))
                                             .ToListAsync();

            return habitaciones.Any()
                ? OperationResult.Success(habitaciones)
                : OperationResult.Failure("No se encontraron habitaciones con la tarifa indicada.");
        }

        private OperationResult ValidarTarifa(Tarifas tarifa)
        {
            return _validator.Validate(tarifa);
        }

        private static void UpdateTarifa(Tarifas target, Tarifas source)
        {
            target.Descripcion = source.Descripcion;
            target.PrecioPorNoche = source.PrecioPorNoche;
            target.Descuento = source.Descuento;
            target.FechaInicio = source.FechaInicio;
            target.FechaFin = source.FechaFin;
            target.IdCategoria = source.IdCategoria;
        }
    }
}
