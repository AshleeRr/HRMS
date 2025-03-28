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
        private readonly IValidator<CreatePisoDto> _validator;

        public PisoServices(IPisoRepository pisoRepository, ILogger<PisoServices> logger, 
                            IHabitacionRepository habitacionRepository, 
                            IValidator<CreatePisoDto> validator)
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
                return OperationResult.Success(pisoDto, "Piso obtenido correctamente");
            });
        }
        
        public async Task<OperationResult> Save(CreatePisoDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validation = _validator.Validate(dto);
                if (!validation.IsSuccess) return validation;
                
                var uniqueValidation = await ValidateUniqueDescripcion(dto);
                if (!uniqueValidation.IsSuccess) return uniqueValidation;
                _logger.LogInformation("Guardando el piso {Descripcion}", dto.Descripcion);

                var estado = MapToEntity(dto);
                
                var result = await _pisoRepository.SaveEntityAsync(estado);
                
                if (result.IsSuccess && result.Data is Piso piso)
                {
                    var estadoDto = MapToDto(piso);
                    return OperationResult.Success(estadoDto, 
                        "Piso guardado correctamente .");
                }
                return result;
            });
        }

        public async Task<OperationResult> Update(UpdatePisoDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validacion = ValidateId(dto.IdPiso, "Para actualizar el piso, el ID debe ser mayor que cero.");
                if(!validacion.IsSuccess) return validacion;
                
                var validation = _validator.Validate(dto);
                if (!validation.IsSuccess) return validation;
                
                var uniqueValidation = await ValidateUniqueDescripcion(dto);
                if (!uniqueValidation.IsSuccess) return uniqueValidation;

                _logger.LogInformation("Actualizando piso con ID: {Id}", dto.IdPiso);
        
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null) 
                    return OperationResult.Failure($"No se encontró el piso con ID {dto.IdPiso}.");
                
                UpdateEntityFromDto(piso, dto);
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

                if (!result.IsSuccess || result.Data == null)
                    return OperationResult.Failure($"No se encontraron pisos con la descripción '{descripcion}'.");

                if (result.Data is IEnumerable<Piso> pisos)
                {
                    var pisosList = pisos.ToList();
                    if (!pisosList.Any())
                        return OperationResult.Failure($"No se encontraron pisos con la descripción '{descripcion}'.");
                        
                    var pisosDto = pisosList.Select(MapToDto).ToList();
                    return OperationResult.Success(pisosDto, "Pisos obtenidos correctamente");
                }
                else if (result.Data is Piso piso)
                {
                    var pisoDto = MapToDto(piso);
                    return OperationResult.Success(pisoDto, "Piso obtenido correctamente");
                }
                
                return OperationResult.Failure($"Formato de respuesta inesperado al buscar pisos con la descripción '{descripcion}'.");
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
        
        private static Piso MapToEntity(CreatePisoDto dto)
            => new Piso 
            { 
                Descripcion = dto.Descripcion, 
                Estado = true 
            };
            
        private static void UpdateEntityFromDto(Piso entity, UpdatePisoDto dto)
        {
            entity.Descripcion = dto.Descripcion;
        }
        private async Task<OperationResult> ValidateUniqueDescripcion(CreatePisoDto dto)
        {
            if (await _pisoRepository.ExistsAsync(e =>
                    e.Descripcion == dto.Descripcion && e.Estado == true))
                return OperationResult.Failure($"Ya existe un piso con la descripción '{dto.Descripcion}'.");
            return OperationResult.Success();
        }
            
        private async Task<bool> TieneHabitacionesAsociadas(int idPiso)
        {
            var habitacionesResult = await _habitacionRepository.GetByPisoAsync(idPiso);

            return habitacionesResult.IsSuccess 
                   && habitacionesResult.Data is IEnumerable<Habitacion> habitaciones 
                   && habitaciones.Any(h => h.Estado == true);
        }
    }
}