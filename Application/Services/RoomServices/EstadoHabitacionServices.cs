using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices
{
    public class EstadoHabitacionService : IEstadoHabitacionService
    {
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly ILogger<EstadoHabitacionService> _logger;
        private readonly IValidator<CreateEstadoHabitacionDto> _validator;

        public EstadoHabitacionService(IEstadoHabitacionRepository estadoHabitacionRepository,
            ILogger<EstadoHabitacionService> logger, IHabitacionRepository habitacionRepository, 
            IValidator<CreateEstadoHabitacionDto> validator)
        {
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _logger = logger;
            _habitacionRepository = habitacionRepository ;
            _validator = validator ;
        }

        public async Task<OperationResult> GetAll()
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Obteniendo todos los estados de habitación");
                var estados = await _estadoHabitacionRepository.GetAllAsync();
                if (!estados.Any())
                    return OperationResult.Failure("No se encontraron estados de habitación.");
                
                var estadosDto = estados.Select(MapToDto).ToList();
                return OperationResult.Success(estadosDto, "Estados obtenidos correctamente");
            });
        }

        public async Task<OperationResult> GetById(int id)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validacion = ValidateId(id, "Para obtener el estado de habitación, el ID debe ser mayor que cero.");
                if (!validacion.IsSuccess) return validacion;

                _logger.LogInformation("Obteniendo estado de habitación con ID: {Id}", id);
                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                if (estado == null)
                {
                    return OperationResult.Failure($"No se encontró el estado con ID {id}.");
                }

                var estadoDto = MapToDto(estado);
                return OperationResult.Success(estadoDto, $"Se ha encontrado el estado con el id {id}");
            });
        }
        
        public async Task<OperationResult> Save(CreateEstadoHabitacionDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validation = _validator.Validate(dto);
                if (!validation.IsSuccess) return validation;
                
                var uniqueValidation = await ValidateUniqueEstado(dto);
                if (!uniqueValidation.IsSuccess) return uniqueValidation;
                _logger.LogInformation("Guardando el estado de habitación: {Descripcion}", dto.Descripcion);

                var estado = MapToEntity(dto);
                
                var result = await _estadoHabitacionRepository.SaveEntityAsync(estado);
                
                if (result.IsSuccess && result.Data is EstadoHabitacion estadoGuardado)
                {
                    var estadoDto = MapToDto(estadoGuardado);
                    return OperationResult.Success(estadoDto, 
                        "La descripción del estado de habitación ha sido guardada correctamente.");
                }
                
                return result;
            });
        }
        
        public async Task<OperationResult> Remove(DeleteEstadoHabitacionDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var valId = ValidateId(dto.IdEstadoHabitacion,
                    "Para eliminar el estado de habitación, el ID debe ser mayor que cero.");
                if (!valId.IsSuccess) return valId;

                _logger.LogInformation("Eliminando estado de habitación con ID: {Id}", dto.IdEstadoHabitacion);
                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
                if (estado == null)
                    return OperationResult.Failure($"No se encontró el estado con ID {dto.IdEstadoHabitacion}");

                if (await _habitacionRepository.ExistsAsync(h => h.IdEstadoHabitacion == dto.IdEstadoHabitacion))
                    return OperationResult.Failure(
                        "No se puede eliminar el estado porque está en uso por habitaciones");

                estado.Estado = false;
                var result = await _estadoHabitacionRepository.UpdateEntityAsync(estado);

                if (result.IsSuccess && result.Data is EstadoHabitacion estadoActualizado)
                {
                    var estadoDto = MapToDto(estadoActualizado);
                    return OperationResult.Success(
                        new
                        {
                            Mensaje = $"Se ha eliminado correctamente el estado de habitación con ID: {dto.IdEstadoHabitacion}",
                            Estado = estadoDto
                        }, "Estado eliminado correctamente");
                }
                
                return result;
            });
        }

        public async Task<OperationResult> Update(UpdateEstadoHabitacionDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var valId = ValidateId(dto.IdEstadoHabitacion,
                    "Para actualizar el estado de habitación, el ID debe ser mayor que cero.");
                if (!valId.IsSuccess) return valId;

                var validation = _validator.Validate(dto);
                if (!validation.IsSuccess) return validation;
                
                var uniqueValidation = await ValidateUniqueEstado(dto);
                if (!uniqueValidation.IsSuccess) return uniqueValidation;

                _logger.LogInformation("Actualizando estado de habitación con ID: {Id}", dto.IdEstadoHabitacion);
                var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
                if (estadoHabitacion == null)
                    return OperationResult.Failure($"No se encontró el estado con ID {dto.IdEstadoHabitacion}.");
            
                UpdateEntityFromDto(estadoHabitacion, dto);
                
                var result = await _estadoHabitacionRepository.UpdateEntityAsync(estadoHabitacion);

                if (result.IsSuccess && result.Data is EstadoHabitacion estadoActualizado)
                {
                    var estadoDto = MapToDto(estadoActualizado);
                    return OperationResult.Success(estadoDto, "Estado actualizado correctamente");
                }
                
                return result;
            });
        }
        
        public async Task<OperationResult> GetEstadoByDescripcion(string descripcion)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(descripcion))
                    return OperationResult.Failure("La descripción del estado no puede estar vacía.");

                _logger.LogInformation("Buscando estados por descripción: {Descripcion}", descripcion);
                var result = await _estadoHabitacionRepository.GetEstadoByDescripcionAsync(descripcion);
                if (!result.IsSuccess || result.Data == null) return result;

                return result.Data switch
                {
                    List<EstadoHabitacion> estados => OperationResult.Success(
                        estados.Select(MapToDto).ToList(), 
                        "Estados encontrados correctamente"),
                    
                    EstadoHabitacion estado => OperationResult.Success(
                        MapToDto(estado), 
                        "Estado encontrado correctamente"),
                    
                    _ => OperationResult.Failure("No se encontraron resultados.")
                };
            });
        }
        private async Task<OperationResult> ValidateUniqueEstado(CreateEstadoHabitacionDto dto)
        {
            if (await _estadoHabitacionRepository.ExistsAsync(e =>
                    e.Descripcion == dto.Descripcion && e.Estado == true))
                return OperationResult.Failure($"Ya existe un estado con la descripción '{dto.Descripcion}'.");
            return OperationResult.Success();
        }
        
        private static EstadoHabitacionDto MapToDto(EstadoHabitacion estado)
            => new EstadoHabitacionDto()
            {
                IdEstadoHabitacion = estado.IdEstadoHabitacion,
                Descripcion = estado.Descripcion
            };

        private static EstadoHabitacion MapToEntity(CreateEstadoHabitacionDto dto)
            => new EstadoHabitacion()
            {
                Descripcion = dto.Descripcion,
            };

        private static void UpdateEntityFromDto(EstadoHabitacion entity, UpdateEstadoHabitacionDto dto)
        {
            entity.Descripcion = dto.Descripcion;
        }
        
        private OperationResult ValidateId(int value, string message)
        {
            return value <= 0
                ? OperationResult.Failure(message)
                : OperationResult.Success();
        }
    }
}