using HRMS.Domain.Base;
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

        public EstadoHabitacionRepository(HRMSContext context, ILogger<EstadoHabitacionRepository> logger) 
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<EstadoHabitacion> GetEntityByIdAsync(int id)
        {
            return (id == 0 ? null : await _context.EstadoHabitaciones
                .FirstOrDefaultAsync(e => e.IdEstadoHabitacion == id && e.Estado == true))!;
        }

        public override async Task<List<EstadoHabitacion>> GetAllAsync()
        {
            return await _context.EstadoHabitaciones
                .Where(e => e.Estado == true)
                .ToListAsync();
        }

        public override async Task<OperationResult> SaveEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            try
            {
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
    }
}