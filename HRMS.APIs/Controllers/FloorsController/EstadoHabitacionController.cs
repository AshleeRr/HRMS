using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoHabitacionController : ControllerBase
    {
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly ILogger<EstadoHabitacionController> _logger;

        public EstadoHabitacionController(
            IEstadoHabitacionRepository estadoHabitacionRepository,
            ILogger<EstadoHabitacionController> logger )
        {
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _logger = logger;
        }

        [HttpGet("GetEstadosHabitacion")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var estadosHabitacion = await _estadoHabitacionRepository.GetAllAsync();
                return Ok(estadosHabitacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estados de habitación");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }

        [HttpGet("GetEstadoHabitacionById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                return estadoHabitacion != null
                    ? Ok(estadoHabitacion)
                    : NotFound("Estado de habitación no encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo estado de habitación {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }
        
        [HttpPut("UpdateEstadoHabitacion")]
        public async Task<IActionResult> Update(int id , [FromBody] EstadoHabitacion estadoHabitacion)
        {
            try
            {
                estadoHabitacion.IdEstadoHabitacion = id;
                var result = await _estadoHabitacionRepository.UpdateEntityAsync(estadoHabitacion);
                return result.IsSuccess
                    ? Ok(result)
                    : StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error actualizando estado de habitación {estadoHabitacion.IdEstadoHabitacion}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }
    }
}
