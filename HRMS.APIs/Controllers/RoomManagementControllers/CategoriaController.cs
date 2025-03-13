using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(ICategoryService categoryService, ILogger<CategoriaController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet("GetAllCategorias")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las categorías");
            var result = await _categoryService.GetAll();

            if (!result.IsSuccess)
                _logger.LogWarning("No se encontraron estados de habitación: {Message}", result.Message);

            return result.IsSuccess
                ? Ok(result.Data)
                : NotFound(new { message = result.Message });
        }

        [HttpGet("GetCategoriaById{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Obteniendo Categoria con ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Solicitud con ID inválido: {Id}", id);
                return BadRequest(new { message = "El ID debe ser mayor que cero." });
            }

            var result = await _categoryService.GetById(id);

            if (!result.IsSuccess)
                _logger.LogWarning("No se encontró la categoria con ID {Id}: {Message}", id, result.Message);

            return result.IsSuccess
                ? Ok(result.Data)
                : NotFound(new { message = result.Message });

        }

        [HttpPost("CreateCategoria")]
        public async Task<IActionResult> Create([FromBody] CreateCategoriaDto dto)
        {
            _logger.LogInformation("Creando una nueva categoria  {Descripcion}", dto.Descripcion);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para crear estado de habitación");
                return BadRequest(ModelState);
            }

            var result = await _categoryService.Save(dto);

            if (!result.IsSuccess)
                _logger.LogWarning("Error al crear la categoria: {Message}", result.Message);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = ((CategoriaDto)result.Data) }, result.Data)
                : BadRequest(new { message = result.Message });
        }

        [HttpPut("UpdateCategoriaById{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.IdCategoria)
                return BadRequest(new ProblemDetails
                {
                    Title = "El ID no coincide",
                    Detail = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud",
                    Status = StatusCodes.Status400BadRequest
                });

            _logger.LogInformation($"Actualizando categoria con ID: {dto.IdCategoria}");
            var result = await _categoryService.Update(dto);

            return result.IsSuccess
                ? Ok(result)
                : result.Message.Contains("No se encontró")
                    ? NotFound(result)
                    : BadRequest(result);
        }

        [HttpDelete("DeleteCategoriaById{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Eliminand la categoria ID: {Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para eliminar estado de habitación");
                return BadRequest(ModelState);
            }

            var dto = new DeleteCategoriaDto() { IdCategoria = id };

            var result = await _categoryService.Remove(dto);

            if (!result.IsSuccess)
                _logger.LogWarning("Error al eliminar la categoria ID {Id}: {Message}",
                    dto.IdCategoria, result.Message);

            return result.IsSuccess
                ? Ok(result.Data)
                : result.Message.Contains("No se encontró")
                    ? NotFound(new { message = result.Message })
                    : BadRequest(new { message = result.Message });
        }

        [HttpGet("GetCategoriaByNombreServicio/{nombreServicio}")]
        public async Task<IActionResult> GetByServicio(string nombreServicio)
        {
            _logger.LogInformation("Buscando categorias con los servicios: {nombreServcio}", nombreServicio);

            if (string.IsNullOrWhiteSpace(nombreServicio))
            {
                _logger.LogWarning("Solicitud con nombre del sercicio vacía");
                return BadRequest(new { message = "El nombre del servicio no puede estar vacía." });
            }

            var result = await _categoryService.GetCategoriaByServicio(nombreServicio);

            if (!result.IsSuccess)
                _logger.LogWarning("No se encontraron categorias con el servicio '{nombreServicio}': {Message}",
                    nombreServicio, result.Message);

            return result.IsSuccess
                ? Ok(result.Data)
                : NotFound(new { message = result.Message });
        }

        [HttpGet("GetCategoriaByDescripcion/{descripcion}")]
        public async Task<IActionResult> GetServiciosByDescripcion(string descripcion)
        {
            _logger.LogInformation("Buscando categorias con descripción: {descripcion}", descripcion);

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                _logger.LogWarning("Solicitud con descripción vacía");
                return BadRequest(new { message = "La descripción no puede estar vacía." });
            }

            var result = await _categoryService.GetCategoriaByDescripcion(descripcion);

            if (!result.IsSuccess)
                _logger.LogWarning("No se encontraron categorias con la descripcion '{nombreServicio}': {Message}",
                    descripcion, result.Message);

            return result.IsSuccess
                ? Ok(result.Data)
                : NotFound(new { message = result.Message });
        }

        [HttpGet("GetHabitacionByCapacidad/{capacidad}")]
        public async Task<IActionResult> GetHabitacionesByCapacidad(int capacidad)
        {
            _logger.LogInformation("Buscando habitacion con capacidad: {capacida}", capacidad);

            if (capacidad <= 0)
                return BadRequest(new OperationResult
                    { IsSuccess = false, Message = "la capacidad debe ser mayor que 0." });

            var result = await _categoryService.GetHabitacionesByCapacidad(capacidad);

            if (!result.IsSuccess)
                _logger.LogWarning("No se encontraron habitacion con la capcidad '{capacidad}': {Message}",
                    capacidad, result.Message);

            return result.IsSuccess
                ? Ok(result.Data)
                : NotFound(new { message = result.Message });
        }

    }
}