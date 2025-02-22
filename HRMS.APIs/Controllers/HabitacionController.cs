using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Models.Models.RoomManagementModel;
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
                return Ok(habitaciones.Select(MapToDto));
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
                    ? Ok(MapToDto(habitacion))
                    : NotFound("Habitación no encontrada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo habitación {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("CreateHabitacion")]
        public async Task<IActionResult> Create([FromBody] HabitacionCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(GetModelErrors());

                var habitacion = new Habitacion
                {
                    Numero = dto.Numero,
                    Detalle = dto.Detalle,
                    Precio = dto.Precio,
                    IdEstadoHabitacion = dto.IdEstadoHabitacion,
                    IdPiso = dto.IdPiso,
                    IdCategoria = dto.IdCategoria
                };

                var result = await _habitacionRepository.SaveEntityAsync(habitacion);

                return result.IsSuccess
                    ? CreatedAtAction(nameof(GetById), new { id = habitacion.IdHabitacion }, MapToDto(habitacion))
                    : BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando habitación");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("UpdateHabitacion/{id}")]  
        public async Task<IActionResult> Update(int id, [FromBody] HabitacionUpdateDto dto)
        {
            try
            {
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
                if (habitacion == null) return NotFound("Habitación no encontrada");

                habitacion.Numero = dto.Numero;
                habitacion.Detalle = dto.Detalle;
                habitacion.Precio = dto.Precio;
                habitacion.IdEstadoHabitacion = dto.IdEstadoHabitacion;
                habitacion.IdPiso = dto.IdPiso;
                habitacion.IdCategoria = dto.IdCategoria;

                var result = await _habitacionRepository.UpdateEntityAsync(habitacion);

                return result.IsSuccess
                    ? Ok(MapToDto(habitacion))
                    : BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error actualizando habitación {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPatch("UpdateHabitacion/{id}/precio")]
        public async Task<IActionResult> UpdatePrecio(int id, [FromBody] PrecioUpdateDto dto)
        {
            try
            {
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
                if (habitacion == null) return NotFound("Habitación no encontrada");

                habitacion.Precio = dto.Precio;

                var result = await _habitacionRepository.UpdateEntityAsync(habitacion);

                return result.IsSuccess
                    ? Ok(MapToDto(habitacion))
                    : BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error actualizando precio de habitación {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        private static HabitacionDto MapToDto(Habitacion entity) => new()
        {
            IdHabitacion = entity.IdHabitacion,
            Numero = entity.Numero,
            Detalle = entity.Detalle,
            Precio = entity.Precio ?? 0,
            IdEstadoHabitacion = entity.IdEstadoHabitacion ?? 0,
            IdPiso = entity.IdPiso ?? 0,
            IdCategoria = entity.IdCategoria ?? 0
        };

        private List<string> GetModelErrors() =>
            ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
    }
}
