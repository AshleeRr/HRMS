using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionController : ApiControllerBase
    {
        private readonly IHabitacionService _habitacionService;

        public HabitacionController(IHabitacionService habitacionService, ILogger<HabitacionController> logger)
        : base(logger)
        {
            _habitacionService = habitacionService;
        }

        /// <summary>
        /// Obtiene todas las habitaciones
        /// </summary>
        /// <returns>Lista de habitaciones</returns>
        [HttpGet("GetAllHabitaciones")] 
        [ProducesResponseType(typeof(IEnumerable<HabitacionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las habitaciones");
            var result = await _habitacionService.GetAll();

            return HandleResponse(result);
        }
        

        /// <summary>
        /// Obtiene una habitación por su ID
        /// </summary>
        /// <param name="id">ID de la habitación</param>
        /// <returns>Datos de la habitación</returns>
        [HttpGet("GetByHabitacion{id}")]
        [ProducesResponseType(typeof(HabitacionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(typeof(HabitacionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateHabitacionDTo dto)
        {
            _logger.LogInformation("Creando nueva habitación");
            
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _habitacionService.Save(dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Error al crear la habitacion: {Message}", result.Message);
                return BadRequest(CreateProblemDetails(result.Message, StatusCodes.Status400BadRequest));
            }

            var habitacionDto = (HabitacionDto)result.Data;
            return CreatedAtAction(nameof(GetById), new { id = habitacionDto.IdHabitacion }, habitacionDto );
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
                return ValidationProblem(ModelState);
            }

            if (id != dto.IdHabitacion)
            {
                return BadRequest(CreateProblemDetails(
                    "El ID en la URL no coincide con el ID en el cuerpo de la solicitud", 
                    StatusCodes.Status400BadRequest));
            }

            var result = await _habitacionService.Update(dto);
            return HandleOperationResult(result);
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

            var validation = ValidateId(id);
            if (validation!= null) return BadRequest(validation);

            var dto = new DeleteHabitacionDto() { IdHabitacion = id };
            
            var result = await _habitacionService.Remove(dto);

            return HandleResponse(result);
        }

        /// <summary>
        /// Obtiene habitaciones por piso
        /// </summary>
        /// <param name="idPiso">ID del piso</param>
        /// <returns>Lista de habitaciones del piso</returns>
        [HttpGet("GetHabitacionByPiso/{idPiso}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByPiso(int idPiso)
        {
            _logger.LogInformation($"Obteniendo habitaciones del piso con ID: {idPiso}");
            
            var validation = ValidateId(idPiso);
            if (validation != null) return validation;
            
            var result = await _habitacionService.GetByPiso(idPiso);
            return HandleResponse(result);
            
        }

        /// <summary>
        /// Obtiene habitaciones por categoría
        /// </summary>
        /// <param name="categoria">Descripción de la categoría</param>
        /// <returns>Lista de habitaciones de la categoría</returns>
        [HttpGet("GetHabitacionByCategoria/{categoria}")]
        [ProducesResponseType(typeof(IEnumerable<HabitacionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCategoria(string categoria)
        {
            _logger.LogInformation($"Obteniendo habitaciones de la categoría: {categoria}");
            
            var validation = ValidateString(categoria, "Categoría");
            if (validation != null) return validation;
            
            var result = await _habitacionService.GetByCategoria(categoria);
            return HandleResponse(result);
        }

        /// <summary>
        /// Obtiene una habitación por su número
        /// </summary>
        /// <param name="numero">Número de la habitación</param>
        /// <returns>Datos de la habitación</returns>
        [HttpGet("GetHabitacionBy/{numero}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNumero(string numero)
        {
            _logger.LogInformation($"Obteniendo habitación con número: {numero}");
            
            var validation = ValidateString(numero, "Número de habitación");
            if (validation != null) return validation;
            
            var result = await _habitacionService.GetByNumero(numero);
            return HandleResponse(result);
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
            return HandleResponse(result);
        }
    }
}