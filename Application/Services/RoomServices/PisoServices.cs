using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices
{
    public class PisoServices : IPisoService
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly ILogger<PisoServices> _logger;
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly IValidator<PisoDto> _validator;

        public PisoServices(IPisoRepository pisoRepository, ILogger<PisoServices> logger, 
                            IHabitacionRepository habitacionRepository, 
                            IValidator<PisoDto> validator)
        {
            _pisoRepository = pisoRepository;
            _logger = logger;
            _habitacionRepository = habitacionRepository;
            _validator = validator;
        }

        public async Task<OperationResult> GetAll()
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando todos los pisos.");
                var pisos = await _pisoRepository.GetAllAsync();
        
                if (pisos?.Any() != true)
                {
                    return OperationResult.Failure("No se encontraron pisos registrados");
                }
        
                var pisosDto = pisos.Select(MapToDto).ToList();
                return OperationResult.Success(pisosDto, "Pisos obtenidos correctamente");
            });
        }

        public async Task<OperationResult> GetById(int id)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validacion = ValidateId(id, "Para obtener el piso, el ID debe ser mayor que cero.");
                if (!validacion.IsSuccess) return validacion;
                
                _logger.LogInformation("Buscando piso por ID: {Id}", id);
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                
                if (piso == null)
                    return OperationResult.Failure($"No se encontró el piso con ID {id}.");
                
                var pisoDto = MapToDto(piso);
                return OperationResult.Success(pisoDto, "Pisos obtenidos correctamente");
            });
        }


        public async Task<OperationResult> Save(CreatePisoDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validation = _validator.Validate(dto);
                if(!validation.IsSuccess) return validation;
                
                if (await _pisoRepository.ExistsAsync(p => p.Descripcion == dto.Descripcion))
                    return OperationResult.Failure($"Ya existe un piso con la descripción '{dto.Descripcion}'.");

                _logger.LogInformation("Creando un nuevo piso.");
                var piso = new Piso { Descripcion = dto.Descripcion, Estado = true };

                var result = await _pisoRepository.SaveEntityAsync(piso);
        
                if (result.IsSuccess && result.Data is Piso pisoCreado)
                {
                    var pisoDto = MapToDto(pisoCreado);
                    return OperationResult.Success(pisoDto, "Piso creado correctamente.");
                }
                
                return OperationResult.Failure(result.Message ?? "Error al guardar el piso.");
            });
        }

        public async Task<OperationResult> Update(UpdatePisoDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validacion = ValidateId(dto.IdPiso, "Para actualizar el piso, el ID debe ser mayor que cero.");
                if(!validacion.IsSuccess) return validacion;
                
                var valitation = _validator.Validate(dto);
                if (!valitation.IsSuccess) return valitation;
                
                _logger.LogInformation("Actualizando piso con ID: {Id}", dto.IdPiso);
        
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null) 
                    return OperationResult.Failure($"No se encontró el piso con ID {dto.IdPiso}.");

                if (await _pisoRepository.ExistsAsync(p => p.Descripcion == dto.Descripcion && p.IdPiso != dto.IdPiso))
                    return OperationResult.Failure($"Ya existe un piso con la descripción '{dto.Descripcion}'.");

                piso.Descripcion = dto.Descripcion;
                var result = await _pisoRepository.UpdateEntityAsync(piso);

                if (result.IsSuccess && result.Data is Piso pisoActualizado)
                {
                    var pisoDto = MapToDto(pisoActualizado);
                    return OperationResult.Success(pisoDto, "Piso actualizado correctamente.");
                }
                
                return OperationResult.Failure(result.Message ?? "Error al actualizar el piso.");
            });
        }

        public async Task<OperationResult> Remove(DeletePisoDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validacion = ValidateId(dto.IdPiso, "Para eliminar el piso, el ID debe ser mayor que cero.");
                if (!validacion.IsSuccess) return validacion;
        
                _logger.LogInformation("Eliminando piso con ID: {Id}", dto.IdPiso);
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null) return OperationResult.Failure($"No se encontró el piso con ID {dto.IdPiso}.");

                var tieneHabitaciones = await TieneHabitacionesAsociadas(dto.IdPiso);
                if (tieneHabitaciones) return OperationResult.Failure(
                        "No se puede eliminar el piso porque tiene habitaciones asociadas. Debe eliminar o reubicar las habitaciones primero.");

                piso.Estado = false;
                var result = await _pisoRepository.UpdateEntityAsync(piso);

                if (result.IsSuccess && result.Data is Piso pisoEliminado)
                {
                    var pisoDto = MapToDto(pisoEliminado);
                    return OperationResult.Success(pisoDto, "Piso eliminado correctamente.");
                }
                
                return OperationResult.Failure("Error al eliminar el piso.");
            });
        }

        public async Task<OperationResult> GetPisoByDescripcion(string descripcion)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(descripcion))
                    return OperationResult.Failure("La descripción no puede estar vacía.");

                _logger.LogInformation("Buscando piso por descripción: {Descripcion}", descripcion);
                var result = await _pisoRepository.GetPisoByDescripcion(descripcion);

                if (result.IsSuccess && result.Data != null)
                {
                    if (result.Data is IEnumerable<Piso> pisos)
                    {
                        var pisosList = pisos.ToList();
                        if (pisosList.Any())
                        {
                            var pisosDto = pisosList.Select(MapToDto).ToList();
                            return OperationResult.Success(pisosDto, "Piso obtenido correctamente");
                        }
                    }
                    else if (result.Data is Piso piso)
                    {
                        var pisoDto = MapToDto(piso);
                        return OperationResult.Success(pisoDto, "Piso obtenido correctamente");
                    }
                }
                return OperationResult.Failure($"No se encontraron pisos con la descripción '{descripcion}'.");
            });
        }
        
        private static OperationResult ValidateId(int id, string message)
        {
            return id <= 0 ? OperationResult.Failure(message) : OperationResult.Success();
        }
        
        private static PisoDto MapToDto(Piso piso)
            => new PisoDto()
            {
                IdPiso = piso.IdPiso, 
                Descripcion = piso.Descripcion
            };
            
        private async Task<bool> TieneHabitacionesAsociadas(int idPiso)
        {
            var habitacionesResult = await _habitacionRepository.GetByPisoAsync(idPiso);

            return habitacionesResult.IsSuccess 
                   && habitacionesResult.Data is IEnumerable<Habitacion> habitaciones 
                   && habitaciones.Any(h => h.Estado == true);
        }
    }
}