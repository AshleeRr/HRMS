using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionController : ControllerBase
    {
        private readonly IHabitacionService _habitacionService;
        private readonly ILogger<HabitacionController> _logger;

        public HabitacionController(IHabitacionService habitacionService, ILogger<HabitacionController> logger)
        {
            _habitacionService = habitacionService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las habitaciones
        /// </summary>
        /// <returns>Lista de habitaciones</returns>
        [HttpGet("GetAllHabitaciones")] 
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las habitaciones");
            var result = await _habitacionService.GetAll();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtiene una habitación por su ID
        /// </summary>
        /// <param name="id">ID de la habitación</param>
        /// <returns>Datos de la habitación</returns>
        [HttpGet("GetBy{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo habitación con ID: {id}");
            var result = await _habitacionService.GetById(id);
            
            if (!result.IsSuccess) return BadRequest(result);
            return result.Data == null ? NotFound(result) : Ok(result);
        }

        /// <summary>
        /// Crea una nueva habitación
        /// </summary>
        /// <param name="dto">Datos de la habitación a crear</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("CreateHabitacion")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateHabitacionDTo dto)
        {
            _logger.LogInformation("Creando nueva habitación");
            var result = await _habitacionService.Save(dto);
            
            return result.IsSuccess 
                ? CreatedAtAction(nameof(GetById), new { id = ((dynamic)result.Data).IdHabitacion }, result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Actualiza una habitación existente
        /// </summary>
        /// <param name="id">ID de la habitación</param>
        /// <param name="dto">Datos actualizados de la habitación</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("(UpdateHabitacionBy){id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHabitacionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(id != dto.IdHabitacion)
                return BadRequest(new ProblemDetails { 
                    Title = "El ID no coincide", 
                    Detail = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud",
                    Status = StatusCodes.Status400BadRequest
                });
            
            _logger.LogInformation($"Actualizando habitacion con ID: {dto.IdPiso}");
            var result = await _habitacionService.Update(dto);
            
            return result.IsSuccess 
                ? Ok(result) 
                : result.Message.Contains("No se encontró") 
                    ? NotFound(result) 
                    : BadRequest(result);
        }

        /// <summary>
        /// Elimina una habitación (baja lógica)
        /// </summary>
        /// <param name="id">ID de la habitación a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("DeleteHabitacionBy{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Eliminando habitación con ID: {id}");
            var dto = new DeleteHabitacionDto { IdHabitacion = id };
            var result = await _habitacionService.Remove(dto);
            
            if (!result.IsSuccess) return BadRequest(result);
            return result.Data == null ? NotFound(result) : Ok(result);
        }

        /// <summary>
        /// Obtiene habitaciones por piso
        /// </summary>
        /// <param name="idPiso">ID del piso</param>
        /// <returns>Lista de habitaciones del piso</returns>
        [HttpGet("GetHabitacionByPiso/{idPiso}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByPiso(int idPiso)
        {
            _logger.LogInformation($"Obteniendo habitaciones del piso con ID: {idPiso}");
            var result = await _habitacionService.GetByPiso(idPiso);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtiene habitaciones por categoría
        /// </summary>
        /// <param name="categoria">Descripción de la categoría</param>
        /// <returns>Lista de habitaciones de la categoría</returns>
        [HttpGet("GetHabitacionByCategoria/{categoria}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCategoria(string categoria)
        {
            _logger.LogInformation($"Obteniendo habitaciones de la categoría: {categoria}");
            var result = await _habitacionService.GetByCategoria(categoria);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtiene una habitación por su número
        /// </summary>
        /// <param name="numero">Número de la habitación</param>
        /// <returns>Datos de la habitación</returns>
        [HttpGet("GetHabitacionBy/{numero}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByNumero(string numero)
        {
            _logger.LogInformation($"Obteniendo habitación con número: {numero}");
            var result = await _habitacionService.GetByNumero(numero);
            
            if (!result.IsSuccess) return BadRequest(result);
            return result.Data == null ? NotFound(result) : Ok(result);
        }

        /// <summary>
        /// Obtiene información detallada de todas las habitaciones
        /// </summary>
        /// <returns>Información detallada de habitaciones</returns>
        [HttpGet("GetInfoHabitaciones")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoHabitaciones()
        {
            _logger.LogInformation("Obteniendo información detallada de habitaciones");
            var result = await _habitacionService.GetInfoHabitacionesAsync();
            
            if (!result.IsSuccess) return BadRequest(result);
            return result.Data == null ? NotFound(result) : Ok(result);
        }
    }
}