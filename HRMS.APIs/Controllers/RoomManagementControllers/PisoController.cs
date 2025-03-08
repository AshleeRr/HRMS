using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PisoController : ControllerBase
    {
        private readonly IPisoService _pisoService;
        private readonly ILogger<PisoController> _logger;

        public PisoController(IPisoService pisoService, ILogger<PisoController> logger)
        {
            _pisoService = pisoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los pisos
        /// </summary>
        /// <returns>Lista de pisos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los pisos");
            var result = await _pisoService.GetAll();
            
            return result.IsSuccess 
                ? Ok(result) 
                : NotFound(result);
        }

        /// <summary>
        /// Obtiene un piso por su ID
        /// </summary>
        /// <param name="id">ID del piso</param>
        /// <returns>Piso encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo piso por ID: {id}");
            var result = await _pisoService.GetById(id);
            
            return result.IsSuccess 
                ? Ok(result) 
                : NotFound(result);
        }

        /// <summary>
        /// Busca un piso por su descripción
        /// </summary>
        /// <param name="descripcion">Descripción del piso</param>
        /// <returns>Piso encontrado</returns>
        [HttpGet("por-descripcion/{descripcion}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByDescripcion(string descripcion)
        {
            _logger.LogInformation($"Buscando piso por descripción: {descripcion}");
            var result = await _pisoService.GetPisoByDescripcion(descripcion);
            
            return result.IsSuccess 
                ? Ok(result) 
                : NotFound(result);
        }

        /// <summary>
        /// Crea un nuevo piso
        /// </summary>
        /// <param name="dto">Datos del piso a crear</param>
        /// <returns>Piso creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreatePisoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creando nuevo piso");
            var result = await _pisoService.Save(dto);
            
            return result.IsSuccess 
                ? Created($"api/piso/{result.Data?.GetType().GetProperty("IdPiso")?.GetValue(result.Data, null)}", result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Actualiza un piso existente
        /// </summary>
        /// <param name="dto">Datos del piso a actualizar</param>
        /// <returns>Piso actualizado</returns>
        [HttpPut]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] UpdatePisoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Actualizando piso con ID: {dto.IdPiso}");
            var result = await _pisoService.Update(dto);
            
            return result.IsSuccess 
                ? Ok(result) 
                : result.Message.Contains("No se encontró") 
                    ? NotFound(result) 
                    : BadRequest(result);
        }

        /// <summary>
        /// Elimina un piso (cambio de estado)
        /// </summary>
        /// <param name="dto">Datos del piso a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromBody] DeletePisoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Eliminando piso con ID: {dto.IdPiso}");
            var result = await _pisoService.Remove(dto);
            
            return result.IsSuccess 
                ? Ok(result) 
                : result.Message.Contains("No se encontró") 
                    ? NotFound(result) 
                    : BadRequest(result);
        }
    }
}