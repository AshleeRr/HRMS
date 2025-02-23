using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(
            ICategoryRepository categoryRepository,
            ILogger<CategoriaController> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        [HttpGet("GetCategorias")]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _categoryRepository.GetAllAsync();
            return Ok(categorias);
        }
        
        [HttpGet("GetCategoriaById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var categoria = await _categoryRepository.GetEntityByIdAsync(id);
                return categoria != null
                    ? Ok(categoria)
                    : NotFound("Categoria no encontrada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo categoria {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        
        [HttpGet("GetCategoriasByServicio")]
        public async Task<IActionResult> GetByServicios(int idServicio)
        {
            try
            {
                var categorias = await _categoryRepository.GetByServiciosAsync(idServicio);
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo categorías por servicio");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        
        [HttpGet("GetCategoriasActivas")]
        public async Task<IActionResult> GetActivas()
        {
            try
            {
                var categorias = await _categoryRepository.GetActivasAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo categorías activas");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        
        [HttpPost("CreateCategoria")]
        public async Task<IActionResult> Create([FromBody] Categoria categoria)
        {
            try
            {
                var result = await _categoryRepository.SaveEntityAsync(categoria);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }
                return CreatedAtAction(nameof(GetById), new {id = categoria.IdCategoria}, categoria);   

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando categoria");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        [HttpPut("UpdateCategoria/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Categoria categoria)
        {
            try
            {
                var result = await _categoryRepository.UpdateEntityAsync(categoria);
                return result.IsSuccess
                    ? Ok(result.Data)
                    : BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error actualizando categoria {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        
    }
}
