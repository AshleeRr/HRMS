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
        private readonly IPisoRepository _pisoRepository;
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly ICategoryRepository _categoryRepository;


        public HabitacionServices(IHabitacionRepository habitacionRepository,
            ILogger<HabitacionServices> logger,IValidator<CreateHabitacionDTo> habitacionValidator, 
            IReservationRepository reservaRepository, IPisoRepository pisoRepository, IEstadoHabitacionRepository estadoHabitacionRepository, ICategoryRepository categoryRepository)
        {
            _habitacionRepository = habitacionRepository;
            _logger = logger;
            _habitacionValidator = habitacionValidator;
            _reservaRepository = reservaRepository;
            _pisoRepository = pisoRepository;
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _categoryRepository = categoryRepository;
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
            try
            {
                _logger.LogInformation($"Creando habitación con número: {dto.Numero}");

                if (await _habitacionRepository.ExistsAsync(e => e.Numero == dto.Numero))
                    return OperationResult.Failure($"Ya existe una habitación con el número {dto.Numero}");
        
                var validationDto = _habitacionValidator.Validate(dto);
                if (!validationDto.IsSuccess) return validationDto;
                
                var foreignKeyValidation = await ValidateForeignKeys(dto.IdPiso,
                    dto.IdCategoria, dto.IdEstadoHabitacion);
                if (!foreignKeyValidation.IsSuccess)
                    return foreignKeyValidation;
        
                var habitacion = MapToEntity(dto);
                habitacion.Estado = true;

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
                
                var foreignKeyValidation = await ValidateForeignKeys(dto.IdPiso, dto.IdCategoria, dto.IdEstadoHabitacion);
                if (!foreignKeyValidation.IsSuccess)
                    return foreignKeyValidation;

                UpdateEntityFromDto(habitacion, dto);
        
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
                if (resultado.Data == null)
                    return OperationResult.Success(null, "No se encontró la información de las habitaciones");
        
                var habitacionesInfo = resultado.Data is IEnumerable<dynamic> habitacionesDatos
                    ? MapToHabitacionInfoDtoList(habitacionesDatos)
                    : null;
        
                return OperationResult.Success(habitacionesInfo, "Información de las habitaciones obtenida correctamente");
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
        
        private static Habitacion MapToEntity(CreateHabitacionDTo dto)
            => new Habitacion()
            {
                Numero = dto.Numero,
                Detalle = dto.Detalle,
                Precio = dto.Precio,
                IdCategoria = dto.IdCategoria,
                IdPiso = dto.IdPiso,
                IdEstadoHabitacion = dto.IdEstadoHabitacion
            };

        private static void UpdateEntityFromDto(Habitacion entity, UpdateHabitacionDto dto)
        {
            if (dto.Numero != null) entity.Numero = dto.Numero;
            if (dto.Detalle != null) entity.Detalle = dto.Detalle;
            if (dto.Precio.HasValue) entity.Precio = dto.Precio.Value;
            if (dto.IdCategoria.HasValue) entity.IdCategoria = dto.IdCategoria.Value;
            if (dto.IdPiso.HasValue) entity.IdPiso = dto.IdPiso.Value;
            if (dto.IdEstadoHabitacion.HasValue) entity.IdEstadoHabitacion = dto.IdEstadoHabitacion.Value;
        }

        private List<HabitacionInfoDto> MapToHabitacionInfoDtoList(IEnumerable<dynamic> habitacionesDatos)
        {
            if (habitacionesDatos == null)
                return new List<HabitacionInfoDto>();

            var result = new List<HabitacionInfoDto>();

            foreach (var item in habitacionesDatos)
            {
                try
                {
                    if (item == null)
                        continue;

                    var dto = new HabitacionInfoDto
                    {
                        IdHabitacion = GetPropertyValue<int>(item, "IdHabitacion"),
                        Numero = GetPropertyValue<string>(item, "Numero") ?? string.Empty,
                        Detalle = GetPropertyValue<string>(item, "Detalle") ?? string.Empty,
                        PrecioPorNoche = GetPropertyValue<decimal>(item, "PrecioPorNoche"),
                        DescripcionPiso = GetPropertyValue<string>(item, "DescripcionPiso") ?? string.Empty,
                        DescripcionCategoria = GetPropertyValue<string>(item, "DescripcionCategoria") ?? string.Empty,
                        NombreServicio = GetPropertyValue<string>(item, "NombreServicio") ?? "Sin servicio",
                        DescripcionServicio = GetPropertyValue<string>(item, "DescripcionServicio") ?? "Sin descripción"
                    };

                    result.Add(dto);
                }
                catch (Exception)
                {
                    _logger.LogWarning("Error al mapear la información de la habitación");
                }
            }

            return result;
        }

        /// <summary>
        /// Obtiene el valor de una propiedad de un objeto de forma segura, independientemente de su tipo.
        /// Funciona tanto con objetos dinámicos (ExpandoObject) como con objetos tipados.
        /// </summary>
        /// <typeparam name="T">El tipo de valor que se espera obtener</typeparam>
        /// <param name="obj">El objeto del cual se extraerá la propiedad</param>
        /// <param name="propertyName">El nombre de la propiedad a obtener</param>
        /// <returns>El valor de la propiedad convertido al tipo T, o el valor por defecto si no se encuentra</returns>
        private T GetPropertyValue<T>(object obj, string propertyName)
        {
            try
            {
                // CASO 1: Si el objeto es un diccionario (como ExpandoObject o tipos dinámicos)
                if (obj is IDictionary<string, object> dict && dict.TryGetValue(propertyName, out var val))
                {
                    // Si el valor obtenido es nulo, devolver el valor por defecto para el tipo T
                    if (val == null)
                        return default;

                    // Si el valor ya es del tipo T, devolverlo directamente
                    // Si no, intentar convertirlo al tipo T
                    return val is T typedVal ? typedVal : (T)Convert.ChangeType(val, typeof(T));
                }

                // CASO 2: Para objetos normales o anónimos, usar reflexión
                // Buscar la propiedad por nombre (ignorando mayúsculas/minúsculas)
                var prop = obj.GetType().GetProperty(propertyName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.IgnoreCase);

                // Si la propiedad no existe, devolver el valor por defecto
                if (prop == null)
                    return default;

                // Obtener el valor de la propiedad
                var value = prop.GetValue(obj);

                // Si el valor es nulo, devolver el valor por defecto
                if (value == null)
                    return default;

                // Si el valor ya es del tipo T, devolverlo directamente
                // Si no, intentar convertirlo al tipo T
                return value is T typedValue ? typedValue : (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                // Si ocurre cualquier excepción (propiedad no encontrada, error de conversión, etc.)
                // devolver el valor por defecto para el tipo T
                return default;
            }

        }

        private async Task<OperationResult> ValidateForeignKeys(int? idPiso, int? idCategoria, int? idEstadoHabitacion)
        {
            if (!idPiso.HasValue)
                return OperationResult.Failure("El ID del piso no puede ser nulo.");
    
            if (!idCategoria.HasValue)
                return OperationResult.Failure("El ID de la categoría no puede ser nulo.");
    
            if (!idEstadoHabitacion.HasValue)
                return OperationResult.Failure("El ID del estado de habitación no puede ser nulo.");

            bool pisoExists = await _pisoRepository.ExistsAsync(p => p.IdPiso == idPiso.Value && p.Estado == true);
            if (!pisoExists)
                return OperationResult.Failure($"El piso con ID {idPiso} no existe o está inactivo.");
    
            bool categoriaExists = await _categoryRepository.ExistsAsync(c => c.IdCategoria == idCategoria.Value && c.Estado == true);
            if (!categoriaExists)
                return OperationResult.Failure($"La categoría con ID {idCategoria} no existe o está inactiva.");
    
            bool estadoExists = await _estadoHabitacionRepository.ExistsAsync(e => 
                e.IdEstadoHabitacion == idEstadoHabitacion.Value && e.Estado == true);
            if (!estadoExists)
                return OperationResult.Failure($"El estado de habitación con ID {idEstadoHabitacion} no existe o está inactivo.");

            return OperationResult.Success();
        }
    }
}