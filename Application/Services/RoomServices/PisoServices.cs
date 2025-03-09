using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices
{
    public class PisoServices : IPisoService
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly ILogger<PisoServices> _logger;
        private readonly IReservationRepository _reservaRepository;
        private readonly IHabitacionRepository _habitacionRepository;

        public PisoServices(IPisoRepository pisoRepository, ILogger<PisoServices> logger, 
                            IReservationRepository reservaRepository, IHabitacionRepository habitacionRepository)
        {
            _pisoRepository = pisoRepository;
            _logger = logger;
            _reservaRepository = reservaRepository;
            _habitacionRepository = habitacionRepository;
        }

        public async Task<OperationResult> GetAll()
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando todos los pisos.");
                var pisos = await _pisoRepository.GetAllAsync();

                return pisos.Any() 
                    ? OperationResult.Success(pisos.Select(MapToDto)) 
                    : OperationResult.Failure("No se encontraron pisos.");
            });
        }

        public async Task<OperationResult> GetById(int id)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (id <= 0) return OperationResult.Failure("El ID del piso debe ser mayor que 0.");

                _logger.LogInformation("Buscando piso por ID: {Id}", id);
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                
                return piso != null 
                    ? OperationResult.Success(MapToDto(piso)) 
                    : OperationResult.Failure($"No se encontró el piso con ID {id}.");
            });
        }

        public async Task<OperationResult> Save(CreatePisoDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (dto == null)
                    return OperationResult.Failure("Los datos del piso no pueden ser nulos.");
                
                var validacion = ValidarDescripcion(dto.Descripcion);
                if (!validacion.IsSuccess) return validacion;

                if (await _pisoRepository.ExistsAsync(p => p.Descripcion == dto.Descripcion))
                    return OperationResult.Failure($"Ya existe un piso con la descripción '{dto.Descripcion}'.");

                _logger.LogInformation("Creando un nuevo piso.");
                var piso = new Piso { Descripcion = dto.Descripcion, Estado = true };

                var result = await _pisoRepository.SaveEntityAsync(piso);
        
                return result.IsSuccess && result.Data != null
                    ? OperationResult.Success(MapToDto((Piso)result.Data), "Piso creado correctamente.")
                    : OperationResult.Failure(result.Message ?? "Error al guardar el piso.");
            });
        }

        public async Task<OperationResult> Update(UpdatePisoDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (dto == null)
                    return OperationResult.Failure("Los datos del piso no pueden ser nulos.");
            
                if (dto.IdPiso <= 0)
                    return OperationResult.Failure("El ID del piso debe ser mayor que 0.");
            
                var validacion = ValidarDescripcion(dto.Descripcion);
                if (!validacion.IsSuccess) return validacion;

                _logger.LogInformation("Actualizando piso con ID: {Id}", dto.IdPiso);
        
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null) 
                    return OperationResult.Failure($"No se encontró el piso con ID {dto.IdPiso}.");

                if (await _pisoRepository.ExistsAsync(p => p.Descripcion == dto.Descripcion && p.IdPiso != dto.IdPiso))
                    return OperationResult.Failure($"Ya existe un piso con la descripción '{dto.Descripcion}'.");

                piso.Descripcion = dto.Descripcion;
                var result = await _pisoRepository.UpdateEntityAsync(piso);

                return result.IsSuccess && result.Data != null
                    ? OperationResult.Success(MapToDto((Piso)result.Data), "Piso actualizado correctamente.")
                    : OperationResult.Failure(result.Message ?? "Error al actualizar el piso.");
            });
        }

        public async Task<OperationResult> Remove(DeletePisoDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (dto.IdPiso <= 0) return OperationResult.Failure("El ID del piso debe ser mayor que 0.");
                _logger.LogInformation("Eliminando piso con ID: {Id}", dto.IdPiso);
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null) return OperationResult.Failure($"No se encontró el piso con ID {dto.IdPiso}.");

                if (await TieneReservasActivas(dto.IdPiso))
                {
                    return OperationResult.Failure(
                        "No se puede eliminar el piso porque tiene reservas activas asociadas.");
                }

                if (await TieneHabitacionesAsociadas(dto.IdPiso))
                {
                    return OperationResult.Failure(
                        "No se puede eliminar el piso porque tiene habitaciones asociadas. Debe eliminar o reubicar las habitaciones primero.");
                }

                piso.Estado = false;
                var result = await _pisoRepository.UpdateEntityAsync(piso);

                return result.IsSuccess && result.Data != null
                    ? OperationResult.Success(MapToDto((Piso)result.Data), "Piso eliminado correctamente.")
                    : OperationResult.Failure("Error al eliminar el piso.");
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
                            return OperationResult.Success(pisosList.Select(MapToDto));
                        }
                    }
                    else if (result.Data is Piso piso)
                    {
                        return OperationResult.Success(MapToDto(piso));
                    }
                }

                return OperationResult.Failure($"No se encontraron pisos con la descripción '{descripcion}'.");
            });
        }

        private async Task<bool> TieneReservasActivas(int idPiso)
        {
            var habitacionesResult = await _habitacionRepository.GetByPisoAsync(idPiso);

            if (!habitacionesResult.IsSuccess || habitacionesResult.Data == null) return false;

            var habitaciones = habitacionesResult.Data as IEnumerable<Habitacion>;
            if (habitaciones == null || !habitaciones.Any()) return false;

            var fechaActual = DateTime.Now;
            foreach (var habitacion in habitaciones)
            {
                var reservasResult = await _reservaRepository.GetAllAsync(
                    r => r.IdHabitacion == habitacion.IdHabitacion && r.FechaSalida >= fechaActual && r.Estado == true);

                if (reservasResult.IsSuccess && reservasResult.Data is IEnumerable<object> reservas && reservas.Any())
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> TieneHabitacionesAsociadas(int idPiso)
        {
            var habitacionesResult = await _habitacionRepository.GetByPisoAsync(idPiso);

            if (!habitacionesResult.IsSuccess || habitacionesResult.Data == null) return false;

            var habitaciones = habitacionesResult.Data as IEnumerable<Habitacion>;
    
            return habitaciones != null && habitaciones.Any(h => h.Estado == true);
        }

        private static OperationResult ValidarDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return OperationResult.Failure("La descripción es requerida.");
            if (descripcion.Length > 50)
                return OperationResult.Failure("La descripción no puede exceder los 50 caracteres.");
            return OperationResult.Success();
        }

        private static PisoDto MapToDto(Piso piso) => new()
        {
            Descripcion = piso.Descripcion,
        };
    }
}
