using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoHabitacionController : ApiControllerBase
    {
        private readonly IEstadoHabitacionService _estadoHabitacionService;

        public EstadoHabitacionController(IEstadoHabitacionService estadoHabitacionService,
            ILogger<EstadoHabitacionController> logger)
        : base(logger)
        {
            _estadoHabitacionService = estadoHabitacionService;
        }

        /// <summary>
        /// Obtiene todos los estados de habitación activos
        /// </summary>
        /// <returns>Lista de estados de habitación</returns>
        [HttpGet("GetEstadoHabitaciones")]
        [ProducesResponseType(typeof(IEnumerable<EstadoHabitacionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los estados de habitación");
            var result = await _estadoHabitacionService.GetAll();
            
            return HandleResponse(result);
        }

        /// <summary>
        /// Obtiene un estado de habitación por su ID
        /// </summary>
        /// <param name="id">ID del estado de habitación</param>
        /// <returns>Estado de habitación</returns>
        [HttpGet("GetEstadoBy(id){id}")]
        [ProducesResponseType(typeof(EstadoHabitacionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Obteniendo estado de habitación con ID: {Id}", id);
            
            var validation = ValidateId(id);
            if (validation != null) return validation;
            
            var result = await _estadoHabitacionService.GetById(id);
            return HandleResponse(result);
        }

        /// <summary>
        /// Busca estados de habitación por descripción
        /// </summary>
        /// <param name="descripcion">Descripción a buscar</param>
        /// <returns>Estado(s) de habitación que coinciden con la descripción</returns>
        [HttpGet("GetEstadoBy(descripcion){descripcion}")]
        [ProducesResponseType(typeof(IEnumerable<EstadoHabitacionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByDescripcion([FromQuery] string descripcion)
        {
            _logger.LogInformation("Buscando estados de habitación con descripción: {Descripcion}", descripcion);
            
            var validation = ValidateString(descripcion , "Estado habitacion Descripción");
            if (validation != null) return validation;
            
            var result = await _estadoHabitacionService.GetEstadoByDescripcion(descripcion);
            return HandleResponse(result);
        }

        /// <summary>
        /// Crea un nuevo estado de habitación
        /// </summary>
        /// <param name="dto">Datos del estado de habitación</param>
        /// <returns>Estado de habitación creado</returns>
        [HttpPost("CreateEstadoHabitacion")]
        [ProducesResponseType(typeof(EstadoHabitacionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateEstadoHabitacionDto dto)
        {
            _logger.LogInformation("Creando un nuevo estado de habitacion {Descripcion}", dto.Descripcion);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _estadoHabitacionService.Save(dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Error al crear el estado: {Message}", result.Message);
                return BadRequest(CreateProblemDetails(result.Message, StatusCodes.Status400BadRequest));
            }

            var estadoHabitacionDto = (EstadoHabitacionDto)result.Data;
            return CreatedAtAction(nameof(GetById), new { id = estadoHabitacionDto.IdEstadoHabitacion }, estadoHabitacionDto);
        }
        

        /// <summary>
        /// Actualiza un estado de habitación
        /// </summary>
        /// <param name="dto">Datos del estado de habitación</param>
        /// <returns>Estado de habitación actualizado</returns>
        [HttpPatch("UpdateEstadoHabitacionById{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEstadoHabitacionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id != dto.IdEstadoHabitacion)
            {
                return BadRequest(CreateProblemDetails(
                    "El ID en la URL no coincide con el ID en el cuerpo de la solicitud", 
                    StatusCodes.Status400BadRequest));
            }

            var result = await _estadoHabitacionService.Update(dto);
            return HandleOperationResult(result);
        }

        /// <summary>
        /// Elimina lógicamente un estado de habitación
        /// </summary>
        /// <param name="id">Datos del estado de habitación a eliminar</param>
        /// <returns>Estado de habitación eliminado</returns>
        [HttpDelete("DeleteEstadoHabitacionById{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Eliminando el estado con ID: {Id}", id);
            
            var validation = ValidateId(id);
            if (validation != null) return validation;
            
            var dto = new DeleteEstadoHabitacionDto() { IdEstadoHabitacion = id };
            var result = await _estadoHabitacionService.Remove(dto);
            
            return HandleOperationResult(result);
        }
    }
}