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
    public class EstadoHabitacionRepository : BaseRepository<EstadoHabitacion, int>, IEstadoHabitacionRepository
    {
        private readonly ILogger<EstadoHabitacionRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IValidator<EstadoHabitacion> _validator;

        public EstadoHabitacionRepository(HRMSContext context, ILogger<EstadoHabitacionRepository> logger,
            IConfiguration configuration, IValidator<EstadoHabitacion> validator) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
        }

        public override async Task<EstadoHabitacion> GetEntityByIdAsync(int id)
        {
            return id == 0 ? null : await _context.EstadoHabitaciones
                .FirstOrDefaultAsync(e => e.IdEstadoHabitacion == id && e.Estado == true);
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
                var validation = _validator.Validate(estadoHabitacion);
                if (!validation.IsSuccess) return validation;

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
            var validation = ValidateEstado(estadoHabitacion);
            if (!validation.IsSuccess) return validation;

            var existingEstado = await GetExistingEstadoAsync(estadoHabitacion.IdEstadoHabitacion);
            if (existingEstado == null)
            {
                return OperationResult.Failure("El estado de habitación no existe.");
            }

            var uniqueDescripcionResult = await ValidateUniqueDescripcionForUpdateAsync(estadoHabitacion);
            if (!uniqueDescripcionResult.IsSuccess) return uniqueDescripcionResult;
            
            
            return await ExecuteUpdateOperationAsync(existingEstado, estadoHabitacion);
        }


        private OperationResult ValidateEstado(EstadoHabitacion estadoHabitacion)
        {
            var validation = _validator.Validate(estadoHabitacion);
            return validation.IsSuccess ? new OperationResult { IsSuccess = true } : validation;
        }


        
        private async Task<OperationResult> ValidateUniqueDescripcionForUpdateAsync(EstadoHabitacion estadoHabitacion)
        {
            if (await _context.EstadoHabitaciones.AnyAsync(e =>
                    e.Descripcion == estadoHabitacion.Descripcion &&
                    e.IdEstadoHabitacion != estadoHabitacion.IdEstadoHabitacion))
            {
                return OperationResult.Failure($"Ya existe un estado de habitación con la descripción '{estadoHabitacion.Descripcion}'.");
            }

            return new OperationResult { IsSuccess = true };
        }
        
        private async Task<EstadoHabitacion?> GetExistingEstadoAsync(int idEstadoHabitacion)
        {
            return await _context.EstadoHabitaciones.FindAsync(idEstadoHabitacion);
        }

        
        private async Task<OperationResult> ExecuteUpdateOperationAsync(
            EstadoHabitacion existingEstado, EstadoHabitacion updatedEstado)
        {
            try
            {
                existingEstado.Descripcion = updatedEstado.Descripcion;
                existingEstado.Estado = updatedEstado.Estado;
                await _context.SaveChangesAsync();

                return OperationResult.Success( existingEstado, "Estado de habitación actualizado correctamente.");
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure("El estado de habitación fue modificado por otro usuario. Intente nuevamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando el estado de habitación.");
                return OperationResult.Failure($"Ocurrió un error actualizando el estado de habitación: {ex.Message}");
            }
        }
        
    }
}