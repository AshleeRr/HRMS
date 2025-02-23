using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoHabitacion : ControllerBase
    {
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly ILogger<EstadoHabitacion> _logger;

        public EstadoHabitacion(
            IEstadoHabitacionRepository estadoHabitacionRepository,
            ILogger<EstadoHabitacion> logger)
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
        
    }
}
