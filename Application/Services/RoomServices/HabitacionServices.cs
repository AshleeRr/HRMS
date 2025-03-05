using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices
{
    public class HabitacionServices : IHabitacionService
    {
        private readonly ILogger<HabitacionServices> _logger;
        private readonly IHabitacionRepository _habitacionRepository;

        public HabitacionServices(IHabitacionRepository habitacionRepository, ILogger<HabitacionServices> logger)
        {
            _habitacionRepository = habitacionRepository;
            _logger = logger;
        }
        

        public async Task<OperationResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Obteniendo todas las habitaciones");
                var habitaciones = await _habitacionRepository.GetAllAsync();
                
                return SuccessResult(
                    habitaciones?.Any() == true ? "Habitaciones obtenidas correctamente" : "No se encontraron habitaciones registradas",
                    habitaciones ?? new List<Habitacion>());
            }
            catch (Exception ex)
            {
                return ErrorResult("Error al obtener todas las habitaciones", ex);
            }
        }

        public async Task<OperationResult> GetById(int id)
        {
            if (id <= 0) return ValidationResult("El id de la habitación no puede ser menor o igual a 0");
            try
            {
                _logger.LogInformation($"Obteniendo habitación por id: {id}");
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
                return habitacion == null
                    ? SuccessResult("No se encontró la habitación", null)
                    : SuccessResult("Habitación obtenida correctamente", habitacion);
            }
            catch (Exception ex)
            {
                return ErrorResult($"Error al obtener la habitación con id {id}", ex);
            }
        }

        public async Task<OperationResult> Save(CreateHabitacionDTo dto)
        {
            if (dto == null) return ValidationResult("No se proporcionaron datos para la habitación");

            if (!ValidateHabitacion(dto, out var validationResult)) return validationResult;

            try
            {
                _logger.LogInformation($"Creando habitación con número: {dto.Numero}");

                if ((await _habitacionRepository.GetByNumeroAsync(dto.Numero)).IsSuccess)
                    return ValidationResult($"Ya existe una habitación con el número {dto.Numero}");

                var habitacion = new Habitacion
                {
                    Numero = dto.Numero,
                    Detalle = dto.Detalle,
                    Precio = dto.Precio,
                    IdEstadoHabitacion = dto.IdEstadoHabitacion,
                    IdPiso = dto.IdPiso,
                    IdCategoria = dto.IdCategoria
                };

                var saveResult = await _habitacionRepository.SaveEntityAsync(habitacion);
                return saveResult.IsSuccess ? SuccessResult("Habitación creada correctamente", saveResult.Data) : saveResult;
            }
            catch (Exception ex)
            {
                return ErrorResult("Error al guardar la habitación", ex);
            }
        }

        public async Task<OperationResult> Update(UpdateHabitacionDto dto)
        {
            if (!ValidateDto(dto, out var validationResult)) return validationResult;

            try
            {
                _logger.LogInformation($"Actualizando habitación con ID: {dto.IdHabitacion}");

                var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.IdHabitacion);
                if (habitacion == null) return ValidationResult("La habitación a actualizar no existe");

                if (!await ValidateNumeroUnico(dto, habitacion)) return ValidationResult($"Ya existe otra habitación con el número {dto.Numero}");

                UpdateHabitacionFields(habitacion, dto);
                return await SaveUpdatedHabitacion(habitacion);
            }
            catch (Exception ex)
            {
                return ErrorResult($"Error al actualizar la habitación con ID {dto.IdHabitacion}", ex);
            }
        }

        public async Task<OperationResult> Remove(DeleteHabitacionDto dto)
        {
            if (dto == null || dto.IdHabitacion <= 0) return ValidationResult("ID de habitación inválido");

            try
            {
                _logger.LogInformation($"Eliminando habitación con ID: {dto.IdHabitacion}");

                var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.IdHabitacion);
                if (habitacion == null) return ValidationResult("La habitación no existe");

                habitacion.Estado = false;
                _logger.LogInformation($"Habitación eliminada correctamente");
                return await SaveUpdatedHabitacion(habitacion);
            }
            catch (Exception ex)
            {
                return ErrorResult($"Error al eliminar la habitación con ID {dto.IdHabitacion}", ex);
            }
        }
        
        
        public async Task<OperationResult> GetByPiso(int idPiso)
        {
            try
            {
                _logger.LogInformation("Obteniendo habitaciones por piso");
        
                if (idPiso <= 0) 
                    return ValidationResult("El id del piso no puede ser menor o igual a 0");
        
                var operationResult = await _habitacionRepository.GetByPisoAsync(idPiso);

                var habitaciones = operationResult?.Data as List<Habitacion>; 
                
                return habitaciones?.Any() == true
                    ? SuccessResult("Habitaciones obtenidas correctamente", habitaciones)
                    : SuccessResult("No se encontraron habitaciones registradas", new List<Habitacion>());
            }
            catch (Exception ex)
            {
                return ErrorResult($"Error al obtener las habitaciones del piso {idPiso}", ex);
            }
        }
        public async Task<OperationResult> GetByCategoria(string categoria)
        {
            try
            {
                _logger.LogInformation($"Obteniendo habitaciones por categoría: {categoria}");
                if (string.IsNullOrEmpty(categoria)) return ValidationResult
                    ("La descripción de la categoría no puede estar vacía");
                
                var operationResult = await _habitacionRepository.GetByCategoriaAsync(categoria);
                
                var habitaciones = operationResult?.Data as List<Habitacion>;
                return habitaciones?.Any() == true
                    ? SuccessResult("Habitaciones obtenidas correctamente", habitaciones)
                    : SuccessResult($"No se encontraron habitaciones en la categoría '{categoria}'", new List<Habitacion>());
            }
            catch (Exception ex)
            {
                return ErrorResult($"Error al obtener las habitaciones de la categoría {categoria}", ex);
            }
        }


        public async Task<OperationResult> GetByNumero(string numero)
        {
            try
            {
                _logger.LogInformation($"Obteniendo habitación por número: {numero}");
                if(string.IsNullOrEmpty(numero)) return 
                    ValidationResult("El número de habitación no puede estar vacío");

                var habitacion = await _habitacionRepository.GetByNumeroAsync(numero);
                return habitacion.Data == null
                    ? SuccessResult("No se encontró la habitación", null)
                    : SuccessResult("Habitación obtenida correctamente", habitacion.Data);
            }
            catch (Exception ex)
            {
                return ErrorResult($"Error al obtener la habitación con número {numero}", ex);
            }
        }


        public async Task<OperationResult> GetInfoHabitacionesAsync()
        {
            try
            {
                _logger.LogInformation("Obteniedo información de las habitaciones");
                var habitacion = await _habitacionRepository.GetInfoHabitacionesAsync();
                return habitacion.Data == null
                    ? SuccessResult("No se encontró la información de las habitaciones", null)
                    : SuccessResult("Información de las habitaciones obtenida correctamente", habitacion.Data);
            }catch (Exception ex)
            {
                return ErrorResult("Error al obtener la información de las habitaciones", ex);
            }
        }

        private static OperationResult SuccessResult(string message, object? data) => new() { IsSuccess = true, Message = message, Data = data };
        private static OperationResult ValidationResult(string message) => new() { IsSuccess = false, Message = message };
        private static OperationResult ErrorResult(string message, Exception ex)
        {
            return new() { IsSuccess = false, Message = $"{message}: {ex.Message}" };
        }

        private async Task<bool> ValidateNumeroUnico(UpdateHabitacionDto dto, Habitacion habitacion)
        {
            if (!string.IsNullOrWhiteSpace(dto.Numero) && dto.Numero != habitacion.Numero)
            {
                var existingRoom = await _habitacionRepository.GetByNumeroAsync(dto.Numero);
                return existingRoom.Data == null || ((Habitacion)existingRoom.Data).IdHabitacion == dto.IdHabitacion;
            }
            return true;
        }

        private async Task<OperationResult> SaveUpdatedHabitacion(Habitacion habitacion)
        {
            var updateResult = await _habitacionRepository.UpdateEntityAsync(habitacion);
            return updateResult.IsSuccess ? SuccessResult("Operación realizada correctamente", updateResult.Data) : updateResult;
        }

        private bool ValidateHabitacion(CreateHabitacionDTo dto, out OperationResult result)
        {
            if (string.IsNullOrWhiteSpace(dto.Numero))
            {
                result = ValidationResult("El número de habitación es requerido");
                return false;
            }

            if (dto.Precio <= 0)
            {
                result = ValidationResult("El precio debe ser mayor que cero");
                return false;
            }

            result = new OperationResult();
            return true;
        }

        private bool ValidateDto(UpdateHabitacionDto dto, out OperationResult result)
        {
            if (dto == null)
            {
                result = ValidationResult("No se proporcionaron datos para actualizar la habitación");
                return false;
            }

            if (dto.IdHabitacion <= 0)
            {
                result = ValidationResult("El ID de la habitación no es válido");
                return false;
            }

            result = new OperationResult();
            return true;
        }

        private void UpdateHabitacionFields(Habitacion habitacion, UpdateHabitacionDto dto)
        {
            habitacion.Numero = dto.Numero ?? habitacion.Numero;
            habitacion.Detalle = dto.Detalle ?? habitacion.Detalle;
            habitacion.Precio = dto.Precio ?? habitacion.Precio;
            habitacion.IdEstadoHabitacion = dto.IdEstadoHabitacion ?? habitacion.IdEstadoHabitacion;
            habitacion.IdPiso = dto.IdPiso ?? habitacion.IdPiso;
            habitacion.IdCategoria = dto.IdCategoria ?? habitacion.IdCategoria;
        }
    }
}
