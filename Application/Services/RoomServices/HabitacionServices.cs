using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices
{
    public class HabitacionServices : IHabitacionService
    {
        private readonly ILogger<HabitacionServices> _logger;
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly IValidator<CreateHabitacionDTo> _habitacionValidator;
        private readonly IReservationRepository _reservaRepository;

        public HabitacionServices(IHabitacionRepository habitacionRepository,
            ILogger<HabitacionServices> logger,IValidator<CreateHabitacionDTo> habitacionValidator, 
            IReservationRepository reservaRepository)
        {
            _habitacionRepository = habitacionRepository;
            _logger = logger;
            _habitacionValidator = habitacionValidator;
            _reservaRepository = reservaRepository;
        }
        
        public async Task<OperationResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Obteniendo todas las habitaciones");
                var habitaciones = await _habitacionRepository.GetAllAsync();
                
                string message = habitaciones?.Any() == true 
                    ? "Habitaciones obtenidas correctamente" 
                    : "No se encontraron habitaciones registradas";
                
                var habitacionesDto = habitaciones?.Select(MapToDto).ToList() ?? new List<HabitacionDto>();
                return OperationResult.Success(habitacionesDto, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las habitaciones");
                return OperationResult.Failure($"Error al obtener todas las habitaciones: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetById(int id)
        {
            try
            {
                var validation = ValidateId(id, "El ID de la habitación no es válido");
                if (!validation.IsSuccess) return validation;
                
                _logger.LogInformation($"Obteniendo habitación por id: {id}");
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
                
                string message = habitacion == null 
                    ? "No se encontró la habitación" 
                    : "Habitación obtenida correctamente";
                
                var habitacionDto = habitacion != null ? MapToDto(habitacion) : null;
                return OperationResult.Success(habitacionDto, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la habitación con id {id}");
                return OperationResult.Failure($"Error al obtener la habitación con id {id}: {ex.Message}");
            }
        }

        public async Task<OperationResult> Save(CreateHabitacionDTo dto)
        {
            var validation = _habitacionValidator.Validate(dto);
            if (!validation.IsSuccess) return validation;
            
            try
            {
                _logger.LogInformation($"Creando habitación con número: {dto.Numero}");

                if (await _habitacionRepository.ExistsAsync(e => e.Numero == dto.Numero))
                    return OperationResult.Failure($"Ya existe una habitación con el número {dto.Numero}");

                var habitacion = new Habitacion
                {
                    Numero = dto.Numero,
                    Detalle = dto.Detalle,
                    Precio = dto.Precio,
                    IdEstadoHabitacion = dto.IdEstadoHabitacion,
                    IdPiso = dto.IdPiso,
                    IdCategoria = dto.IdCategoria,
                    Estado = true
                };

                var result = await _habitacionRepository.SaveEntityAsync(habitacion);
                
                if (result.IsSuccess && result.Data is Habitacion habitacionCreada)
                {
                    var habitacionDto = MapToDto(habitacionCreada);
                    return OperationResult.Success(habitacionDto, result.Message);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la habitación");
                return OperationResult.Failure($"Error al guardar la habitación: {ex.Message}");
            }
        }

        public async Task<OperationResult> Update(UpdateHabitacionDto dto)
        {
            var validation = ValidateId(dto.IdHabitacion, "El ID de la habitación no es válido");
            if (!validation.IsSuccess) return validation;
            
            var validationDto = _habitacionValidator.Validate(dto);
            if (!validationDto.IsSuccess) return validationDto;
            
            try
            {
                _logger.LogInformation($"Actualizando habitación con ID: {dto.IdHabitacion}");

                var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.IdHabitacion);
                if (habitacion == null) 
                    return OperationResult.Failure("La habitación a actualizar no existe");

                if (!string.IsNullOrWhiteSpace(dto.Numero) && dto.Numero != habitacion.Numero)
                {
                    if (await _habitacionRepository.ExistsAsync(h => h.Numero == dto.Numero && h.IdHabitacion != dto.IdHabitacion))
                        return OperationResult.Failure($"Ya existe otra habitación con el número {dto.Numero}");
                }

                UpdateHabitacionFields(habitacion, dto);
        
                var result = await _habitacionRepository.UpdateEntityAsync(habitacion);
                
                if (result.IsSuccess && result.Data is Habitacion habitacionActualizada)
                {
                    var habitacionDto = MapToDto(habitacionActualizada);
                    return OperationResult.Success(habitacionDto, result.Message);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la habitación con ID {dto.IdHabitacion}");
                return OperationResult.Failure($"Error al actualizar la habitación con ID {dto.IdHabitacion}: {ex.Message}");
            }
        }

        public async Task<OperationResult> Remove(DeleteHabitacionDto dto)
        {
            try
            {
                var validation = ValidateId(dto.IdHabitacion, "El ID de la habitación no es válido");
                if (!validation.IsSuccess) return validation;
                
                var hasReservas = await TieneReservas(dto.IdHabitacion);
                if (hasReservas) return OperationResult.Failure("La habitación tiene reservas activas");
                
                _logger.LogInformation($"Eliminando habitación con ID: {dto.IdHabitacion}");

                var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.IdHabitacion);
                if (habitacion == null) 
                    return OperationResult.Failure("La habitación no existe");

                habitacion.Estado = false;
                
                var updateResult = await _habitacionRepository.UpdateEntityAsync(habitacion);
                
                if (updateResult.IsSuccess && updateResult.Data is Habitacion habitacionEliminada)
                {
                    var habitacionDto = MapToDto(habitacionEliminada);
                    return OperationResult.Success(habitacionDto, "Habitación eliminada correctamente");
                }
                
                return updateResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la habitación con ID {dto.IdHabitacion}");
                return OperationResult.Failure($"Error al eliminar la habitación con ID {dto.IdHabitacion}: {ex.Message}");
            }
        }
        
        public async Task<OperationResult> GetByPiso(int idPiso)
        {
            try
            {
                var validation = ValidateId(idPiso, "El ID del piso debe ser mayor que 0");
                if (!validation.IsSuccess) return validation;
                
                _logger.LogInformation("Obteniendo habitaciones por piso");
                
                var operationResult = await _habitacionRepository.GetByPisoAsync(idPiso);

                if (!operationResult.IsSuccess)
                    return operationResult;
                
                var habitaciones = operationResult.Data as List<Habitacion>; 
                string message = habitaciones?.Any() == true ? 
                    "Habitaciones obtenidas correctamente" : 
                    "No se encontraron habitaciones registradas";
                
                var habitacionesDto = habitaciones?.Select(MapToDto).ToList() ?? new List<HabitacionDto>();
                return OperationResult.Success(habitacionesDto, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las habitaciones del piso {idPiso}");
                return OperationResult.Failure($"Error al obtener las habitaciones del piso {idPiso}: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetByCategoria(string categoria)
        {
            var validation = ValidateString(categoria, "La categoría de la habitación no puede estar vacía");
            if (!validation.IsSuccess) return validation;
            
            try
            {
                _logger.LogInformation($"Obteniendo habitaciones por categoría: {categoria}");
                
                var operationResult = await _habitacionRepository.GetByCategoriaAsync(categoria);
                
                if (!operationResult.IsSuccess)
                    return operationResult;
                
                var habitaciones = operationResult.Data as List<Habitacion>;
                string message = habitaciones?.Any() == true ? 
                    "Habitaciones obtenidas correctamente" : 
                    $"No se encontraron habitaciones en la categoría '{categoria}'";
                
                var habitacionesDto = habitaciones?.Select(MapToDto).ToList() ?? new List<HabitacionDto>();
                return OperationResult.Success(habitacionesDto, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las habitaciones de la categoría {categoria}");
                return OperationResult.Failure($"Error al obtener las habitaciones de la categoría {categoria}: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetByNumero(string numero)
        {
            var validation = ValidateString(numero, "El número de la habitación no puede estar vacío");
            if (!validation.IsSuccess) return validation;
            
            try
            {
                _logger.LogInformation($"Obteniendo habitación por número: {numero}");
                
                var resultado = await _habitacionRepository.GetByNumeroAsync(numero);
                
                if (!resultado.IsSuccess)
                    return resultado;
                
                string message = resultado.Data == null ? 
                    "No se encontró la habitación" : 
                    "Habitación obtenida correctamente";
                
                if (resultado.Data is Habitacion habitacion)
                {
                    var habitacionDto = MapToDto(habitacion);
                    return OperationResult.Success(habitacionDto, message);
                }
                
                return OperationResult.Success(resultado.Data, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la habitación con número {numero}");
                return OperationResult.Failure($"Error al obtener la habitación con número {numero}: {ex.Message}");
            }
        }
        
        private static OperationResult ValidateId(int? id, string message)
        {
            return id <= 0 
                ? OperationResult.Failure(message) 
                : OperationResult.Success();
        }
        
        private static OperationResult ValidateString(string value, string message)
        {
            return string.IsNullOrWhiteSpace(value) 
                ? OperationResult.Failure(message) 
                : OperationResult.Success();
        }

        public async Task<OperationResult> GetInfoHabitacionesAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo información de las habitaciones");
                
                var resultado = await _habitacionRepository.GetInfoHabitacionesAsync();
                
                if (!resultado.IsSuccess)
                    return resultado;
                
                string message = resultado.Data == null ? 
                    "No se encontró la información de las habitaciones" : 
                    "Información de las habitaciones obtenida correctamente";
                
                return OperationResult.Success(resultado.Data, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de las habitaciones");
                return OperationResult.Failure($"Error al obtener la información de las habitaciones: {ex.Message}");
            }
        }
        
        private async Task<bool> TieneReservas(int idReserva)
        {
            try
            {
                return await _reservaRepository.ExistsAsync(r => r.IdRecepcion == idReserva && r.Estado == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar si la habitación tiene reservas activas");
                return false;
            }
        }

        private static HabitacionDto MapToDto(Habitacion habitacion)
            => new HabitacionDto()
            {
                IdHabitacion = habitacion.IdHabitacion,
                Numero = habitacion.Numero,
                Detalle = habitacion.Detalle,
                Precio = habitacion.Precio,
                IdCategoria = habitacion.IdCategoria,
                IdPiso = habitacion.IdPiso,
                IdEstadoHabitacion = habitacion.IdEstadoHabitacion
            };
            
        private void UpdateHabitacionFields(Habitacion habitacion, UpdateHabitacionDto dto)
        {
            if (dto.Numero != null) habitacion.Numero = dto.Numero;
            if (dto.Detalle != null) habitacion.Detalle = dto.Detalle;
            if (dto.Precio.HasValue) habitacion.Precio = dto.Precio.Value;
            if (dto.IdEstadoHabitacion.HasValue) habitacion.IdEstadoHabitacion = dto.IdEstadoHabitacion.Value;
            if (dto.IdPiso.HasValue) habitacion.IdPiso = dto.IdPiso.Value;
            if (dto.IdCategoria.HasValue) habitacion.IdCategoria = dto.IdCategoria.Value;
        }
    }
}