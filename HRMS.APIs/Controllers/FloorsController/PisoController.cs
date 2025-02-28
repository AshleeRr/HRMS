using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PisoController : ControllerBase
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly ILogger<PisoController> _logger;

        public PisoController(
            IPisoRepository pisoRepository,
            ILogger<PisoController> logger)
        {
            _pisoRepository = pisoRepository;
            _logger = logger;
        }

        [HttpGet("GetPisos")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var pisos = await _pisoRepository.GetAllAsync();
                return Ok(pisos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo pisos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("GetPisoById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                return Ok(piso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo piso {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        
        [HttpGet("GetPisosActivos")]
        public async Task<IActionResult> GetByActivo()
        {
            try
            {
                var pisos = await _pisoRepository.GetByActivoAsync();
                return Ok(pisos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo pisos activos");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        
        [HttpPost("CreatePiso")]
        public async Task<IActionResult> Create([FromBody] Piso piso)
        {
            try
            {
                var result = await _pisoRepository.SaveEntityAsync(piso);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                } 
                return CreatedAtAction(nameof(GetById), new { id = piso.IdPiso }, piso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando piso");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
