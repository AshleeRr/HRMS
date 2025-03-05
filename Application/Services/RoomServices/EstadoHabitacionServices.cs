using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices
{
    public class EstadoHabitacionService : IEstadoHabitacionService
    {
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly ILogger<EstadoHabitacionService> _logger;

        public EstadoHabitacionService(IEstadoHabitacionRepository estadoHabitacionRepository, ILogger<EstadoHabitacionService> logger)
        {
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _logger = logger;
        }

        public async Task<OperationResult> GetAll()
        {
            return await ExecuteOperationAsync(async () =>
            {
                var estados = await _estadoHabitacionRepository.GetAllAsync();
                return estados.Any()
                    ? Success(estados.Select(MapToDto))
                    : Failure("No se encontraron estados de habitación.");
            }, "Error al obtener todos los estados de habitación.");
        }

        public async Task<OperationResult> GetById(int id)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (id <= 0) return Failure("El ID del estado de habitación debe ser mayor que cero.");

                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                return estado != null ? Success(MapToDto(estado)) : Failure($"No se encontró el estado con ID {id}.");
            }, $"Error al obtener el estado de habitación con ID {id}.");
        }

        public async Task<OperationResult> Save(CreateEstadoHabitacionDto dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                var validacion = ValidarEstado(dto.Descripcion);
                if (!validacion.IsSuccess) return validacion;

                var estadoHabitacion = new EstadoHabitacion { Descripcion = dto.Descripcion, Estado = true };
                var result = await _estadoHabitacionRepository.SaveEntityAsync(estadoHabitacion);

                return result.IsSuccess && result.Data != null
                    ? Success(MapToDto((EstadoHabitacion)result.Data))
                    : result;
            }, "Error al guardar el estado de habitación.");
        }

        public async Task<OperationResult> Update(UpdateEstadoHabitacionDto dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                var validacion = ValidarEstado(dto.Descripcion);
                if (!validacion.IsSuccess) return validacion;

                var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
                if (estadoHabitacion == null) return Failure($"No se encontró el estado con ID {dto.IdEstadoHabitacion}.");

                estadoHabitacion.Descripcion = dto.Descripcion;
                var result = await _estadoHabitacionRepository.UpdateEntityAsync(estadoHabitacion);

                return result.IsSuccess && result.Data != null
                    ? Success(MapToDto((EstadoHabitacion)result.Data))
                    : result;
            }, $"Error al actualizar el estado de habitación con ID {dto.IdEstadoHabitacion}.");
        }

        public async Task<OperationResult> Remove(DeleteEstadoHabitacionDto dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (dto.IdEstadoHabitacion <= 0) return Failure("El ID del estado de habitación debe ser mayor que cero.");

                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
                if (estado == null) return Failure($"No se encontró el estado con ID {dto.IdEstadoHabitacion}.");

                if (await VerificarHabitacionesConEstado(dto.IdEstadoHabitacion))
                    return Failure("No se puede eliminar el estado porque está en uso.");

                estado.Estado = false;
                var result = await _estadoHabitacionRepository.UpdateEntityAsync(estado);

                return result.IsSuccess && result.Data != null
                    ? Success(MapToDto((EstadoHabitacion)result.Data))
                    : result;
            }, $"Error al eliminar el estado con ID {dto.IdEstadoHabitacion}.");
        }

        public async Task<OperationResult> GetEstadoByDescripcion(string descripcion)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(descripcion))
                    return Failure("La descripción del estado no puede estar vacía.");

                var result = await _estadoHabitacionRepository.GetEstadoByDescripcionAsync(descripcion);
                if (!result.IsSuccess || result.Data == null) return result;

                return result.Data switch
                {
                    List<EstadoHabitacion> estados => Success(estados.Select(MapToDto)),
                    EstadoHabitacion estado => Success(MapToDto(estado)),
                    _ => Failure("No se encontraron resultados.")
                };
            }, $"Error al obtener estado por descripción '{descripcion}'.");
        }

        
        private async Task<bool> VerificarHabitacionesConEstado(int idEstado)
        {
            try
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar habitaciones con estado {IdEstado}", idEstado);
                return false;
            }
        }

        private static OperationResult ValidarEstado(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return Failure("La descripción del estado de habitación es requerida.");
            if (descripcion.Length > 50)
                return Failure("La descripción no puede exceder los 50 caracteres.");
            return Success();
        }

        private static EstadoHabitacionDto MapToDto(EstadoHabitacion estado) => new()
        {
            Descripcion = estado.Descripcion,
        };

        private static OperationResult Success(object data = null) =>
            new() { IsSuccess = true, Data = data };

        private static OperationResult Failure(string message) =>
            new() { IsSuccess = false, Message = message };

        private async Task<OperationResult> ExecuteOperationAsync(Func<Task<OperationResult>> operation, string errorMessage)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                return Failure(errorMessage);
            }
        }

    }
}
