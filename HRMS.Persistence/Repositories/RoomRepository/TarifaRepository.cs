using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;


namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class TarifaRepository : BaseRepository<Tarifas, int>, ITarifaRepository
    {
        private readonly ILogger<TarifaRepository> _logger;
        private readonly IValidator<Tarifas> _validator;

        public TarifaRepository(
            HRMSContext context,
            ILogger<TarifaRepository> logger,
            IConfiguration configuration,
            IValidator<Tarifas> validator
        ) : base(context)
        {
            _logger = logger;
            _validator = validator;
        }

        public override async Task<List<Tarifas>> GetAllAsync()
        {
            _logger.LogInformation("Obteniendo todas las tarifas activas");
            return await _context.Tarifas
                .Where(t => t.Estado == true)
                .ToListAsync();
        }

        public override async Task<Tarifas> GetEntityByIdAsync(int id)
        {
            return (id == 0 ? null : await _context.Tarifas
                .FirstOrDefaultAsync(t => t.IdTarifa == id && t.Estado == true))!;
        }

        
        public override async Task<OperationResult> SaveEntityAsync(Tarifas tarifas)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Guardando nueva tarifa");
                
                var validationResult = _validator.Validate(tarifas);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al guardar tarifa: {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }
                
                await _context.Tarifas.AddAsync(tarifas);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Tarifa guardada correctamente con ID: {Id}", tarifas.IdTarifa);
                return OperationResult.Success(tarifas, "Tarifa guardada correctamente.");
            });
        }

        public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifas)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                
                _logger.LogInformation("Actualizando tarifa con ID: {Id}", tarifas.IdTarifa);
                
                var validationResult = _validator.Validate(tarifas);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al guardar tarifa: {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }

                var existingTarifa = await _context.Tarifas.FindAsync(tarifas.IdTarifa);
                if (existingTarifa == null)
                {
                    _logger.LogWarning("No se encontró la tarifa con ID: {Id}", tarifas.IdTarifa);
                    return OperationResult.Failure($"La tarifa con ID {tarifas.IdTarifa} no existe.");
                }
                
                UpdateTarifa(existingTarifa, tarifas);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Tarifa actualizada correctamente con ID: {Id}", existingTarifa.IdTarifa);
                return OperationResult.Success(existingTarifa, "Tarifa actualizada correctamente.");
            });
        }

        public async Task<OperationResult> GetTarifasVigentesAsync(string fechaInput)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando tarifas vigentes para fecha: {FechaInput}", fechaInput);
                
                if (string.IsNullOrWhiteSpace(fechaInput))
                {
                    _logger.LogWarning("Fecha vacía proporcionada");
                    return OperationResult.Failure("La fecha no puede estar vacía.");
                }
        
                DateTime fecha;
                string[] formatosValidos = { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" };
                string formatosEjemplo = string.Join(", ", formatosValidos);
        
                if (!DateTime.TryParseExact(fechaInput, formatosValidos, 
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out fecha))
                {
                    _logger.LogWarning("Formato de fecha inválido: {FechaInput}", fechaInput);
                    return OperationResult.Failure(
                        $"El formato de la fecha '{fechaInput}' es incorrecto. Debe usar uno de los siguientes formatos: {formatosEjemplo}.");
                }
                var tarifas = await _context.Tarifas
                    .Where(t => t.Estado == true && t.FechaInicio <= fecha && t.FechaFin >= fecha)
                    .ToListAsync();

                if (!tarifas.Any())
                {
                    _logger.LogInformation("No se encontraron tarifas vigentes para la fecha: {Fecha}", fecha.ToString("yyyy-MM-dd"));
                    return OperationResult.Failure($"No se encontraron tarifas vigentes para la fecha {fecha:yyyy-MM-dd}.");
                }
                
                _logger.LogInformation("Se encontraron {Count} tarifas vigentes para la fecha {Fecha}", 
                    tarifas.Count, fecha.ToString("yyyy-MM-dd"));
                return OperationResult.Success(tarifas, "Tarifas encontradas correctamente");
            });
        }

        public async Task<OperationResult> GetHabitacionByPrecioAsync(decimal precio)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando habitaciones con precio de tarifa: {Precio}", precio);
                
                if (precio <= 0)
                {
                    _logger.LogWarning("Precio inválido proporcionado: {Precio}", precio);
                    return OperationResult.Failure("El precio debe ser mayor que cero.");
                }
                
                var fechaActual = DateTime.Now;
                var tarifas = await _context.Tarifas
                    .Where(t => t.PrecioPorNoche == precio && 
                                t.Estado == true &&
                                t.FechaInicio <= fechaActual && 
                                t.FechaFin >= fechaActual)
                    .ToListAsync();

                if (!tarifas.Any())
                {
                    _logger.LogInformation("No se encontraron tarifas vigentes con el precio: {Precio}", precio);
                    return OperationResult.Failure($"No se encontraron tarifas vigentes con el precio {precio:C}.");
                }

                var categoriaIds = tarifas.Select(t => t.IdCategoria).ToList();
                var habitaciones = await _context.Habitaciones
                    .Where(h => h.Estado == true && 
                                h.IdCategoria.HasValue && 
                                categoriaIds.Contains(h.IdCategoria.Value))
                    .ToListAsync();

                if (!habitaciones.Any())
                {
                    _logger.LogInformation("No se encontraron habitaciones con las categorías de las tarifas encontradas");
                    return OperationResult.Failure($"No se encontraron habitaciones con precio de tarifa {precio:C}.");
                }
                
                _logger.LogInformation("Se encontraron {Count} habitaciones con precio de tarifa {Precio}", 
                    habitaciones.Count, precio);
                return OperationResult.Success(habitaciones, $"Se encontraron {habitaciones.Count} habitaciones con precio de tarifa {precio:C}.");
            });
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