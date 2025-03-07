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
            var validation = ValidateEstado(estadoHabitacion);
            if (!validation.IsSuccess) return validation;

            if (await ExistsAsync(e => e.Descripcion == estadoHabitacion.Descripcion))
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ya existe un estado de habitación con la descripción '{estadoHabitacion.Descripcion}'."
                };
            }

            try
            {
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
            if (string.IsNullOrWhiteSpace(descripcionEstado))
            {
                return new OperationResult { IsSuccess = false, Message = "La descripción del estado no puede estar vacía." };
            }

            var estados = await _context.EstadoHabitaciones
                .Where(e => e.Descripcion.Contains(descripcionEstado, StringComparison.OrdinalIgnoreCase) && e.Estado == true)
                .ToListAsync();

            return new OperationResult
            {
                IsSuccess = estados.Any(),
                Message = estados.Any() ? null : $"No se encontró un estado de habitación con la descripción '{descripcionEstado}'.",
                Data = estados
            };
        }

        public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var validation = ValidateEstado(estadoHabitacion);
            if (!validation.IsSuccess) return validation;

            var existingEstado = await _context.EstadoHabitaciones.FindAsync(estadoHabitacion.IdEstadoHabitacion);
            if (existingEstado == null)
            {
                return new OperationResult { IsSuccess = false, Message = "El estado de habitación no existe." };
            }

            if (await _context.EstadoHabitaciones.AnyAsync(e =>
                    e.Descripcion == estadoHabitacion.Descripcion &&
                    e.IdEstadoHabitacion != estadoHabitacion.IdEstadoHabitacion))
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ya existe un estado de habitación con la descripción '{estadoHabitacion.Descripcion}'."
                };
            }

            if (existingEstado.Estado == true && estadoHabitacion.Estado == false)
            {
                var habitacionesAsociadas = await _context.Habitaciones
                    .Where(h => h.IdEstadoHabitacion == existingEstado.IdEstadoHabitacion)
                    .ToListAsync();

                if (habitacionesAsociadas.Any())
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "No se puede desactivar este estado porque hay habitaciones asociadas a él."
                    };
                }
            }

            try
            {
                existingEstado.Descripcion = estadoHabitacion.Descripcion;
                existingEstado.Estado = estadoHabitacion.Estado;
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = estadoHabitacion.Estado == false
                        ? "Estado de habitación desactivado correctamente."
                        : "Estado de habitación actualizado correctamente.",
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
    }
}
