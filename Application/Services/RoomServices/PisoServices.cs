using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices
{
    public class PisoServices : IPisoService
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly ILogger<PisoServices> _logger;

        public PisoServices(IPisoRepository pisoRepository, ILogger<PisoServices> logger)
        {
            _pisoRepository = pisoRepository;
            _logger = logger;
        }


        public async Task<OperationResult> GetAll()
        {
            return await ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando todos los pisos.");
                var pisos = await _pisoRepository.GetAllAsync();
                return pisos.Any() ? Success(pisos.Select(MapToDto)) : Failure("No se encontraron pisos.");
            }, "Error al buscar todos los pisos.");
        }

        public async Task<OperationResult> GetById(int id)
        {
            return await ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando piso por ID: {Id}", id);
                if (id <= 0) return Failure("El ID del piso debe ser mayor que 0.");
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                return piso != null ? Success(MapToDto(piso)) : Failure($"No se encontró el piso con ID {id}.");
            }, $"Error al buscar el piso con ID {id}.");
        }

        public async Task<OperationResult> Save(CreatePisoDto dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                var validacion = ValidarDescripcion(dto.Descripcion);
                if (!validacion.IsSuccess) return validacion;

                _logger.LogInformation("Creando un nuevo piso.");
                var piso = new Piso { Descripcion = dto.Descripcion, Estado = true };

                var result = await _pisoRepository.SaveEntityAsync(piso);
                return result.IsSuccess && result.Data != null
                    ? Success(MapToDto((Piso)result.Data), "Piso creado correctamente.")
                    : result;
            }, "Error al crear el piso.");
        }

        public async Task<OperationResult> Update(UpdatePisoDto dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                var validacion = ValidarDescripcion(dto.Descripcion);
                if (!validacion.IsSuccess) return validacion;

                _logger.LogInformation("Actualizando el piso con ID: {Id}", dto.IdPiso);
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null) return Failure($"No se encontró el piso con ID {dto.IdPiso}.");

                piso.Descripcion = dto.Descripcion;
                var result = await _pisoRepository.UpdateEntityAsync(piso);

                return result.IsSuccess && result.Data != null
                    ? Success(MapToDto((Piso)result.Data), "Piso actualizado correctamente.")
                    : result;
            }, $"Error al actualizar el piso con ID {dto.IdPiso}.");
        }

        public async Task<OperationResult> Remove(DeletePisoDto dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (dto.IdPiso <= 0) return Failure("El ID del piso debe ser mayor que 0.");

                _logger.LogInformation("Eliminando el piso con ID: {Id}", dto.IdPiso);
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null) return Failure($"No se encontró el piso con ID {dto.IdPiso}.");

                piso.Estado = false;
                var result = await _pisoRepository.UpdateEntityAsync(piso);

                return result.IsSuccess && result.Data != null
                    ? Success(MapToDto((Piso)result.Data), "Piso eliminado correctamente.")
                    : result;
            }, $"Error al eliminar el piso con ID {dto.IdPiso}.");
        }

        public async Task<OperationResult> GetPisoByDescripcion(string descripcion)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(descripcion)) return Failure("La descripción del piso no puede estar vacía.");

                _logger.LogInformation("Buscando piso por descripción: {Descripcion}", descripcion);
                var result = await _pisoRepository.GetPisoByDescripcion(descripcion);

                return result.IsSuccess && result.Data != null
                    ? Success(MapToDto((Piso)result.Data))
                    : Failure($"No se encontraron pisos con la descripción '{descripcion}'.");
            }, $"Error al buscar el piso con descripción '{descripcion}'.");
        }

        
        private static OperationResult ValidarDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return Failure("La descripción del piso es requerida.");
            if (descripcion.Length > 50)
                return Failure("La descripción del piso no puede exceder los 50 caracteres.");
            return Success();
        }

        private static PisoDto MapToDto(Piso piso) => new()
        {
            Descripcion = piso.Descripcion,
        };

        private static OperationResult Success(object data = null, string message = null) =>
            new() { IsSuccess = true, Data = data, Message = message };

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
