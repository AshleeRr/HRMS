using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class EstadoHabitacionRepository : BaseRepository<EstadoHabitacion, int>, IEstadoHabitacionRepository
    {
        private readonly ILogger<EstadoHabitacionRepository> _logger;
        private readonly IValidator<EstadoHabitacion> _validator;

        public EstadoHabitacionRepository(HRMSContext context, ILogger<EstadoHabitacionRepository> logger, IValidator<EstadoHabitacion> validator) 
            : base(context)
        {
            _logger = logger;
            _validator = validator;
        }

        public override async Task<EstadoHabitacion> GetEntityByIdAsync(int id)
        {
            _logger.LogInformation($"Obteniendo estado de habitación por ID: {id}");
            return (id == 0 ? null : await _context.EstadoHabitaciones
                .FirstOrDefaultAsync(e => e.IdEstadoHabitacion == id && e.Estado == true))!;
        }

        public override async Task<List<EstadoHabitacion>> GetAllAsync()
        {
            _logger.LogInformation("Obteniendo todos los estados de habitación activos");
            return await _context.EstadoHabitaciones
                .Where(e => e.Estado == true)
                .ToListAsync();
        }

        public override async Task<OperationResult> SaveEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            try
            {
                _logger.LogInformation("Guardando nuevo estado de habitación");
                
                var validationResult = _validator.Validate(estadoHabitacion);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al agregar un : {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }
                
                await _context.EstadoHabitaciones.AddAsync(estadoHabitacion);
                await _context.SaveChangesAsync();
                
                return OperationResult.Success(estadoHabitacion, "Estado de habitación guardado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando el estado de habitación");
                return OperationResult.Failure($"Error al guardar: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetEstadoByDescripcionAsync(string descripcion)
        {
            try
            {
                _logger.LogInformation($"Buscando estados por descripción '{descripcion}'");
                var validationResult = await validateString(descripcion, "La descripción no puede estar vacía.");
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }
                
                var estados = await _context.EstadoHabitaciones
                    .Where(e => e.Descripcion != null && 
                                EF.Functions.Like(e.Descripcion, $"%{descripcion}%") && 
                                e.Estado == true)
                    .ToListAsync();

                return OperationResult.Success(
                    estados, 
                    estados.Any() ? "Estados encontrados correctamente" : $"No se encontraron estados con la descripción '{descripcion}'"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al buscar estados por descripción '{descripcion}'");
                return OperationResult.Failure($"Error al buscar estados: {ex.Message}");
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            try
            {
                _logger.LogInformation($"Actualizando estado de habitación con ID: {estadoHabitacion.IdEstadoHabitacion}");
                
                var validationResult = _validator.Validate(estadoHabitacion);
                
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al actualizar el estado de habitación: {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }
                
                var existingEstado = await _context.EstadoHabitaciones.FindAsync(estadoHabitacion.IdEstadoHabitacion);
                if (existingEstado == null)
                {
                    return OperationResult.Failure("El estado de habitación no existe.");
                }

                existingEstado.Descripcion = estadoHabitacion.Descripcion;
                existingEstado.Estado = estadoHabitacion.Estado;
                await _context.SaveChangesAsync();

                return OperationResult.Success(existingEstado, "Estado de habitación actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando el estado de habitación.");
                return OperationResult.Failure($"Error al actualizar: {ex.Message}");
            }
        }
        private async Task<OperationResult> validateString(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogWarning(message);
                return OperationResult.Failure(message);
            }

            return OperationResult.Success();
        }
    }
}