using HRMS.Application.DTOs.RoomManagementDto.HabitacionDto;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices;

public class HabitacionServices : IHabitacionService
{
    private readonly ILogger _logger;
    private readonly IHabitacionRepository _habitacionRepository;
    private IConfiguration _configuration;

    public HabitacionServices(IHabitacionRepository habitacionRepository, ILogger<HabitacionServices>
        logger, IConfiguration configuration)
    {
        _habitacionRepository = habitacionRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<OperationResult> GetAll()
    {
        OperationResult result = new OperationResult();
        try
        {
            _logger.LogInformation("Obteniendo todas las habitaciones");

            var habitaciones = await _habitacionRepository.GetAllAsync();

            if (habitaciones == null || habitaciones.Any())
            {
                _logger.LogWarning("No se encontraron habitaciones");
                result.IsSuccess = true;
                result.Message = "No se encontraron habitaciones registradas";
                result.Data = new List<Domain.Entities.RoomManagement.Habitacion>();
                return result;
            }
            
            result.IsSuccess = true;
            result.Message = "Habitaciones obtenidas correctamente";
            result.Data = habitaciones;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las habitaciones");
            result.IsSuccess = false;
            result.Message = $"Error al obtener las habitaciones: {ex.Message}";
        }

        return result;
    }


    public async Task<OperationResult> GetById(int id)
    {
        OperationResult result = new OperationResult();
        try
        {
            _logger.LogInformation($"Obteniendo habitación por id: {id}");

            if (id <= 0)
            {
                _logger.LogWarning("El id de la habitación no puede ser menor o igual a 0");
                result.IsSuccess = false;
                result.Message = "El id de la habitación no puede ser menor o igual a 0";
                return result;
            }

            var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);

            if (habitacion == null)
            {
                _logger.LogWarning($"No se encontró habitación con id: {id}");
                result.IsSuccess = true;
                result.Message = "No se encontró la habitación";
                result.Data = null;
                return result;
            }

            result.IsSuccess = true;
            result.Message = "Habitación obtenida correctamente";
            result.Data = habitacion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener la habitación con id {id}");
            result.IsSuccess = false;
            result.Message = $"Error al obtener la habitación: {ex.Message}";
        }

        return result;
    }
    
    public Task<OperationResult> Update(UpdateHabitacionDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult> Save(CreateHabitacionDTo dto)
    {
        var result = new OperationResult();

        try
        {
            _logger.LogInformation("Iniciando creación de habitación");

            if (dto == null)
            {
                _logger.LogWarning("El DTO de habitación es nulo");
                result.IsSuccess = false;
                result.Message = "No se proporcionaron datos para la habitación";
                return result;
            }

            if (string.IsNullOrWhiteSpace(dto.Numero))
            {
                _logger.LogWarning("El número de habitación es requerido");
                result.IsSuccess = false;
                result.Message = "El número de habitación es requerido";
                return result;
            }

            if (dto.Precio <= 0)
            {
                _logger.LogWarning("El precio debe ser mayor que cero");
                result.IsSuccess = false;
                result.Message = "El precio debe ser mayor que cero";
                return result;
            }

            var existingRoom = await _habitacionRepository.GetByNumeroAsync(dto.Numero);
            if (existingRoom.IsSuccess && existingRoom.Data != null)
            {
                _logger.LogWarning($"Ya existe una habitación con el número {dto.Numero}");
                result.IsSuccess = false;
                result.Message = $"Ya existe una habitación con el número {dto.Numero}";
                return result;
            }

            var habitacion = new Domain.Entities.RoomManagement.Habitacion
            {
                Numero = dto.Numero,
                Detalle = dto.Detalle,
                Precio = dto.Precio,
                IdEstadoHabitacion = dto.IdEstadoHabitacion,
                IdPiso = dto.IdPiso,
                IdCategoria = dto.IdCategoria,
            };

            var saveResult = await _habitacionRepository.SaveEntityAsync(habitacion);

            if (!saveResult.IsSuccess)
            {
                _logger.LogWarning($"Error al guardar la habitación: {saveResult.Message}");
                return saveResult; // Devolver el resultado del repositorio
            }

            _logger.LogInformation($"Habitación {dto.Numero} creada exitosamente");

            result.IsSuccess = true;
            result.Message = "Habitación creada correctamente";
            result.Data = saveResult.Data; // Devolver la entidad guardada con su ID asignado
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar la habitación");
            result.IsSuccess = false;
            result.Message = $"Error al guardar la habitación: {ex.Message}";
        }

        return result;
    }

    public Task<OperationResult> Remove(DeleteHabitacionDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> GetByPiso(int idPiso)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> GetByNumero(string numero)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> GetByCategoria(string categoria)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> GetDisponibles(DateTime fechaInicio, DateTime fechaFin)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> CambiarEstado(int idHabitacion, int idEstadoHabitacion)
    {
        throw new NotImplementedException();
    }
}