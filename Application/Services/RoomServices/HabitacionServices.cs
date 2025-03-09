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
                
                string message = habitaciones?.Any() == true 
                    ? "Habitaciones obtenidas correctamente" 
                    : "No se encontraron habitaciones registradas";
                
                return OperationResult.Success(habitaciones ?? new List<Habitacion>(), message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las habitaciones");
                return OperationResult.Failure($"Error al obtener todas las habitaciones: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetById(int id)
        {
            if (id <= 0) 
                return OperationResult.Failure("El id de la habitación no puede ser menor o igual a 0");
            
            try
            {
                _logger.LogInformation($"Obteniendo habitación por id: {id}");
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
                
                string message = habitacion == null 
                    ? "No se encontró la habitación" 
                    : "Habitación obtenida correctamente";
                
                return OperationResult.Success(habitacion, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la habitación con id {id}");
                return OperationResult.Failure($"Error al obtener la habitación con id {id}: {ex.Message}");
            }
        }

        public async Task<OperationResult> Save(CreateHabitacionDTo dto)
        {
            if (dto == null) 
                return OperationResult.Failure("No se proporcionaron datos para la habitación");

            if (string.IsNullOrWhiteSpace(dto.Numero))
                return OperationResult.Failure("El número de habitación es requerido");

            if (dto.Precio <= 0)
                return OperationResult.Failure("El precio debe ser mayor que cero");

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

                return await _habitacionRepository.SaveEntityAsync(habitacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la habitación");
                return OperationResult.Failure($"Error al guardar la habitación: {ex.Message}");
            }
        }

        public async Task<OperationResult> Update(UpdateHabitacionDto dto)
        {
            if (dto == null)
                return OperationResult.Failure("No se proporcionaron datos para actualizar la habitación");

            if (dto.IdHabitacion <= 0)
                return OperationResult.Failure("El ID de la habitación no es válido");

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
        
                return await _habitacionRepository.UpdateEntityAsync(habitacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la habitación con ID {dto.IdHabitacion}");
                return OperationResult.Failure($"Error al actualizar la habitación con ID {dto.IdHabitacion}: {ex.Message}");
            }
        }

        public async Task<OperationResult> Remove(DeleteHabitacionDto dto)
        {
            if (dto == null || dto.IdHabitacion <= 0) 
                return OperationResult.Failure("ID de habitación inválido");

            try
            {
                _logger.LogInformation($"Eliminando habitación con ID: {dto.IdHabitacion}");

                var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.IdHabitacion);
                if (habitacion == null) 
                    return OperationResult.Failure("La habitación no existe");

                habitacion.Estado = false;
                
                var updateResult = await _habitacionRepository.UpdateEntityAsync(habitacion);
                return updateResult.IsSuccess ? 
                    OperationResult.Success(updateResult.Data, "Habitación eliminada correctamente") : 
                    updateResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la habitación con ID {dto.IdHabitacion}");
                return OperationResult.Failure($"Error al eliminar la habitación con ID {dto.IdHabitacion}: {ex.Message}");
            }
        }
        
        public async Task<OperationResult> GetByPiso(int idPiso)
        {
            if (idPiso <= 0) 
                return OperationResult.Failure("El id del piso no puede ser menor o igual a 0");
        
            try
            {
                _logger.LogInformation("Obteniendo habitaciones por piso");
                
                var operationResult = await _habitacionRepository.GetByPisoAsync(idPiso);

                if (!operationResult.IsSuccess)
                    return operationResult;
                
                var habitaciones = operationResult.Data as List<Habitacion>; 
                string message = habitaciones?.Any() == true ? 
                    "Habitaciones obtenidas correctamente" : 
                    "No se encontraron habitaciones registradas";
                
                return OperationResult.Success(habitaciones ?? new List<Habitacion>(), message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las habitaciones del piso {idPiso}");
                return OperationResult.Failure($"Error al obtener las habitaciones del piso {idPiso}: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetByCategoria(string categoria)
        {
            if (string.IsNullOrEmpty(categoria)) 
                return OperationResult.Failure("La descripción de la categoría no puede estar vacía");
                
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
                
                return OperationResult.Success(habitaciones ?? new List<Habitacion>(), message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las habitaciones de la categoría {categoria}");
                return OperationResult.Failure($"Error al obtener las habitaciones de la categoría {categoria}: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetByNumero(string numero)
        {
            if(string.IsNullOrEmpty(numero)) 
                return OperationResult.Failure("El número de habitación no puede estar vacío");

            try
            {
                _logger.LogInformation($"Obteniendo habitación por número: {numero}");
                
                var resultado = await _habitacionRepository.GetByNumeroAsync(numero);
                
                if (!resultado.IsSuccess)
                    return resultado;
                
                string message = resultado.Data == null ? 
                    "No se encontró la habitación" : 
                    "Habitación obtenida correctamente";
                
                return OperationResult.Success(resultado.Data, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la habitación con número {numero}");
                return OperationResult.Failure($"Error al obtener la habitación con número {numero}: {ex.Message}");
            }
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