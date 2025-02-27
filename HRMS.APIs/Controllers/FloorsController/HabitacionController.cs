using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionController : ControllerBase
    {
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly ILogger<HabitacionController> _logger;

        public HabitacionController(
            IHabitacionRepository habitacionRepository,
            ILogger<HabitacionController> logger)
        {
            _habitacionRepository = habitacionRepository;
            _logger = logger;
        }

        [HttpGet("GetHabitaciones")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var habitaciones = await _habitacionRepository.GetAllAsync();
                return Ok(habitaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo habitaciones");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("GetHabitacionesById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
                return habitacion != null
                    ? Ok(habitacion)
                    : NotFound("Habitación no encontrada");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo habitación {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("CreateHabitacion")]
        public async Task<IActionResult> Create([FromBody] Habitacion habitacion)
        {
            try
            {
                var result = await _habitacionRepository.SaveEntityAsync(habitacion);
                return result.IsSuccess
                    ? Ok(result.Data)
                    : BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando habitación");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("UpdateHabitacion/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Habitacion habitacion)
        {
            try
            {
                habitacion.IdHabitacion = id;
                var result = await _habitacionRepository.UpdateEntityAsync(habitacion);
                return result.IsSuccess
                    ? Ok(result.Data)
                    : BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error actualizando habitación {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
