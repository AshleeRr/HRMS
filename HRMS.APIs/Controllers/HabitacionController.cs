using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Models.Models.HabitacionModel;
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
        // GET: api/<HabitacionController>
        
        public HabitacionController(IHabitacionRepository habitacionRepository , ILogger<HabitacionController> logger)
        {
            _habitacionRepository = habitacionRepository;
            _logger = logger;
        }
        [HttpGet("GetHabitaciones")]
        public async Task<ActionResult> Get()
        {
            var habitaciones = await _habitacionRepository.GetAllAsync();
            return Ok(habitaciones);
        }
    
        // GET api/<HabitacionController>/5
        [HttpGet("GetHabitacionById")]
        public async Task<ActionResult>Get(int id)
        {
            var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
            return Ok(habitacion);
        }

        // POST api/<HabitacionController>
        [HttpPost("PostHabitacion")]
        public async Task<ActionResult> Post([FromBody] HabitacionModel request)
        {
            try
            {
                var habitacion = new Habitacion
                {
                    Numero = request.Numero,
                    Detalle = request.Detalle,
                    Precio = request.Precio,
                    IdEstadoHabitacion = request.IdEstadoHabitacion,
                    IdPiso = request.IdPiso,
                    IdCategoria = request.IdCategoria
                };
                var result = await _habitacionRepository.SaveEntityAsync(habitacion);

                if (!result.IsSuccess)
                { 
                    return BadRequest(result);
                }
                return Ok(result);
                    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la habitación");
                return BadRequest("Error al guardar la habitación");
            }
        }
        [HttpPost("UpdateRoomPrices")]

        public async Task<ActionResult> UpdatePrice([FromBody] UpdateRoomPrice request)
        {
            try
            {
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(request.IdHabitacion);
                if (habitacion == null)
                {
                    return NotFound(new OperationResult 
                    { 
                        IsSuccess = false, 
                        Message = "Habitación no encontrada" 
                    });
                }

                habitacion.Precio = request.Precio;
                var result = await _habitacionRepository.UpdateEntityAsync(habitacion);

                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el precio de la habitación");
                return StatusCode(500, new OperationResult 
                { 
                    IsSuccess = false, 
                    Message = "Error interno del servidor" 
                });
            }
        }
        [HttpPut("UpdateHabitacion")]
        public async Task<ActionResult> Update([FromBody] UpdateRoom request)
        {
            try
            {
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(request.IdHabitacion);
                if (habitacion == null)
                {
                    return NotFound(new OperationResult 
                    { 
                        IsSuccess = false, 
                        Message = "Habitación no encontrada" 
                    });
                }

                // Actualizamos solo los campos proporcionados
                if (!string.IsNullOrEmpty(request.Numero)) habitacion.Numero = request.Numero;
                if (!string.IsNullOrEmpty(request.Detalle)) habitacion.Detalle = request.Detalle;
                if (request.Precio.HasValue) habitacion.Precio = request.Precio.Value;
                if (request.IdEstadoHabitacion.HasValue) habitacion.IdEstadoHabitacion = request.IdEstadoHabitacion;
                if (request.IdPiso.HasValue) habitacion.IdPiso = request.IdPiso;
                if (request.IdCategoria.HasValue) habitacion.IdCategoria = request.IdCategoria;

                var result = await _habitacionRepository.UpdateEntityAsync(habitacion);

                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la habitación");
                return StatusCode(500, new OperationResult 
                { 
                    IsSuccess = false, 
                    Message = "Error interno del servidor" 
                });
            }
        }
    }
}
