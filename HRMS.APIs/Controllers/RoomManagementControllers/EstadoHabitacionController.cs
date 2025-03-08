using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoHabitacionController : ControllerBase
    {
        private readonly IEstadoHabitacionService _estadoHabitacionService;
        private readonly ILogger<EstadoHabitacionController> _logger;

        public EstadoHabitacionController(IEstadoHabitacionService estadoHabitacionService, ILogger<EstadoHabitacionController> logger)
        {
            _estadoHabitacionService = estadoHabitacionService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los estados de habitación activos
        /// </summary>
        /// <returns>Lista de estados de habitación</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los estados de habitación");
            var result = await _estadoHabitacionService.GetAll();
            
            if (!result.IsSuccess)
                _logger.LogWarning("No se encontraron estados de habitación: {Message}", result.Message);
                
            return result.IsSuccess 
                ? Ok(result.Data) 
                : NotFound(new { message = result.Message });
        }

        /// <summary>
        /// Obtiene un estado de habitación por su ID
        /// </summary>
        /// <param name="id">ID del estado de habitación</param>
        /// <returns>Estado de habitación</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Obteniendo estado de habitación con ID: {Id}", id);
            
            if (id <= 0)
            {
                _logger.LogWarning("Solicitud con ID inválido: {Id}", id);
                return BadRequest(new { message = "El ID debe ser mayor que cero." });
            }

            var result = await _estadoHabitacionService.GetById(id);
            
            if (!result.IsSuccess)
                _logger.LogWarning("No se encontró el estado de habitación con ID {Id}: {Message}", id, result.Message);
                
            return result.IsSuccess 
                ? Ok(result.Data) 
                : NotFound(new { message = result.Message });
        }

        /// <summary>
        /// Busca estados de habitación por descripción
        /// </summary>
        /// <param name="descripcion">Descripción a buscar</param>
        /// <returns>Estado(s) de habitación que coinciden con la descripción</returns>
        [HttpGet("buscar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByDescripcion([FromQuery] string descripcion)
        {
            _logger.LogInformation("Buscando estados de habitación con descripción: {Descripcion}", descripcion);
            
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                _logger.LogWarning("Solicitud con descripción vacía");
                return BadRequest(new { message = "La descripción no puede estar vacía." });
            }

            var result = await _estadoHabitacionService.GetEstadoByDescripcion(descripcion);
            
            if (!result.IsSuccess)
                _logger.LogWarning("No se encontraron estados con la descripción '{Descripcion}': {Message}", 
                    descripcion, result.Message);
                
            return result.IsSuccess 
                ? Ok(result.Data) 
                : NotFound(new { message = result.Message });
        }

        /// <summary>
        /// Crea un nuevo estado de habitación
        /// </summary>
        /// <param name="dto">Datos del estado de habitación</param>
        /// <returns>Estado de habitación creado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateEstadoHabitacionDto dto)
        {
            _logger.LogInformation("Creando nuevo estado de habitación: {Descripcion}", dto.Descripcion);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para crear estado de habitación");
                return BadRequest(ModelState);
            }

            var result = await _estadoHabitacionService.Save(dto);
            
            if (!result.IsSuccess)
                _logger.LogWarning("Error al crear estado de habitación: {Message}", result.Message);
                
            return result.IsSuccess 
                ? CreatedAtAction(nameof(GetById), new { id = ((EstadoHabitacionDto)result.Data) }, result.Data) 
                : BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Actualiza un estado de habitación
        /// </summary>
        /// <param name="dto">Datos del estado de habitación</param>
        /// <returns>Estado de habitación actualizado</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] UpdateEstadoHabitacionDto dto)
        {
            _logger.LogInformation("Actualizando estado de habitación ID: {Id}", dto.IdEstadoHabitacion);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para actualizar estado de habitación");
                return BadRequest(ModelState);
            }

            var result = await _estadoHabitacionService.Update(dto);
            
            if (!result.IsSuccess)
                _logger.LogWarning("Error al actualizar estado de habitación ID {Id}: {Message}", 
                    dto.IdEstadoHabitacion, result.Message);
                
            return result.IsSuccess 
                ? Ok(result.Data) 
                : result.Message.Contains("No se encontró") 
                    ? NotFound(new { message = result.Message }) 
                    : BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Elimina lógicamente un estado de habitación
        /// </summary>
        /// <param name="dto">Datos del estado de habitación a eliminar</param>
        /// <returns>Estado de habitación eliminado</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromBody] DeleteEstadoHabitacionDto dto)
        {
            _logger.LogInformation("Eliminando estado de habitación ID: {Id}", dto.IdEstadoHabitacion);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para eliminar estado de habitación");
                return BadRequest(ModelState);
            }

            var result = await _estadoHabitacionService.Remove(dto);
            
            if (!result.IsSuccess)
                _logger.LogWarning("Error al eliminar estado de habitación ID {Id}: {Message}", 
                    dto.IdEstadoHabitacion, result.Message);
                
            return result.IsSuccess 
                ? Ok(result.Data) 
                : result.Message.Contains("No se encontró") 
                    ? NotFound(new { message = result.Message }) 
                    : BadRequest(new { message = result.Message });
        }
    }
}