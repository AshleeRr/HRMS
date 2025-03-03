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

        public override async Task<EstadoHabitacion?> GetEntityByIdAsync(int id)
        {
            return id != 0 ? await _context.Set<EstadoHabitacion>().FindAsync(id) : null;
        }

        public override Task<List<EstadoHabitacion>> GetAllAsync()
        {
            return _context.EstadoHabitaciones.Where(e => e.Estado == true).ToListAsync();
        }

        public override async Task<OperationResult> SaveEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var result = ValidateEstado(estadoHabitacion);
            if (!result.IsSuccess) return result;

            try
            {
                if (await ExistsAsync(e => e.Descripcion == estadoHabitacion.Descripcion))
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = $"Ya existe un estado de habitación con la descripción '{estadoHabitacion.Descripcion}'."
                    };
                }

                await _context.EstadoHabitaciones.AddAsync(estadoHabitacion);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Estado de habitación guardado correctamente.",
                    Data = estadoHabitacion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando el estado de habitación.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error guardando el estado de habitación: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult> GetEstadoByDescripcionAsync(string descripcionEstado)
        {
            return await GetByFilterAsync(
                "La descripción del estado no puede estar vacía.",
                descripcionEstado,
                _context.EstadoHabitaciones.Where(e => e.Descripcion == descripcionEstado && e.Estado == true),
                $"No se encontró un estado de habitación con la descripción '{descripcionEstado}'."
            );
        }

        public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var result = ValidateEstado(estadoHabitacion);
            if (!result.IsSuccess) return result;

            try
            {
                var existingEstado = await _context.EstadoHabitaciones.FindAsync(estadoHabitacion.IdEstadoHabitacion);
                if (existingEstado == null)
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "El estado de habitación no existe."
                    };
                }

                if (await _context.EstadoHabitaciones.AnyAsync(e => e.Descripcion == estadoHabitacion.Descripcion && e.IdEstadoHabitacion != estadoHabitacion.IdEstadoHabitacion))
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = $"Ya existe un estado de habitación con la descripción '{estadoHabitacion.Descripcion}'."
                    };
                }

                UpdateEstado(existingEstado, estadoHabitacion);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Estado de habitación actualizado correctamente.",
                    Data = existingEstado
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "El estado de habitación fue modificado por otro usuario. Intente nuevamente."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando el estado de habitación.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error actualizando el estado de habitación: {ex.Message}"
                };
            }
        }
        
        private OperationResult ValidateEstado(EstadoHabitacion estadoHabitacion)
        {
            var validation = _validator.Validate(estadoHabitacion);
            return validation.IsSuccess ? new OperationResult { IsSuccess = true } : validation;
        }

        private void UpdateEstado(EstadoHabitacion existing, EstadoHabitacion updated)
        {
            existing.Descripcion = updated.Descripcion;
            existing.Estado = updated.Estado;
        }

        private async Task<OperationResult> GetByFilterAsync(
            string validationMessage, string? filterValue,
            IQueryable<EstadoHabitacion> query, string notFoundMessage)
        {
            if (!string.IsNullOrWhiteSpace(validationMessage) && string.IsNullOrWhiteSpace(filterValue))
            {
                return new OperationResult { IsSuccess = false, Message = validationMessage };
            }

            try
            {
                var resultList = await query.ToListAsync();
                return new OperationResult
                {
                    IsSuccess = true,
                    Message = resultList.Any() ? null : notFoundMessage,
                    Data = resultList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo datos.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error obteniendo datos: {ex.Message}"
                };
            }
        }
    }
}
