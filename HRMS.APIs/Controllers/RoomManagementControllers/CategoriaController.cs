using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.RoomManagementControllers
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
            return HandleOperationResult(result);
        }

        [HttpGet("GetCategoriaById{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Obteniendo categoría con ID: {Id}", id);
            var result = await _categoryService.GetById(id);
            return HandleOperationResult(result);
        }

        [HttpPost("CreateCategoria")]
        public async Task<IActionResult> Create([FromBody] CreateCategoriaDto dto)
        {
            _logger.LogInformation("Creando nueva categoría");
            var result = await _categoryService.Save(dto);
            return HandleOperationResult(result, StatusCodes.Status201Created);
        }

        [HttpPut("UpdateCategoriaById{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateCategoriaDto dto)
        {
            _logger.LogInformation("Actualizando categoría con ID: {Id}", dto.IdCategoria);
            var result = await _categoryService.Update(dto);
            return HandleOperationResult(result);
        }

        [HttpDelete("DeleteCategoriaById{id}")]
        public async Task<IActionResult> Delete([FromBody] DeleteCategoriaDto dto)
        {
            _logger.LogInformation("Eliminando categoría con ID: {Id}", dto.IdCategoria);
            var result = await _categoryService.Remove(dto);
            return HandleOperationResult(result);
        }

        [HttpGet("GetCategoriaByDescripcionServicio/{nombreServicio}")]
        public async Task<IActionResult> GetByServicio(string nombreServicio)
        {
            _logger.LogInformation("Buscando categorías para el servicio: {NombreServicio}", nombreServicio);
            var result = await _categoryService.GetCategoriaByServicio(nombreServicio);
            return HandleOperationResult(result);
        }

        [HttpGet("GetCategoriaByDescripcion/{descripcion}")]
        public async Task<IActionResult> GetServiciosByDescripcion(string descripcion)
        {
            _logger.LogInformation("Buscando servicios con descripción: {Descripcion}", descripcion);
            var result = await _categoryService.GetServiciosByDescripcion(descripcion);
            return HandleOperationResult(result);
        }

        [HttpGet("GetHabitacionByCapacidad/{capacidad}")]
        public async Task<IActionResult> GetHabitacionesByCapacidad(int capacidad)
        {
            _logger.LogInformation("Buscando habitaciones con capacidad: {Capacidad}", capacidad);
            var result = await _categoryService.GetHabitacionesByCapacidad(capacidad);
            return HandleOperationResult(result);
        }

        private IActionResult HandleOperationResult(OperationResult result, int successStatusCode = StatusCodes.Status200OK)
        {
            if (result.IsSuccess)
            {
                return StatusCode(successStatusCode, result);
            }
            
            if (result.Message?.Contains("No se encontró") == true)
            {
                return NotFound(result);
            }
            
            return BadRequest(result);
        }
    }
}