using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarifaController : ControllerBase
    {
        private readonly ITarifaService _tarifaService;
        private readonly ILogger<TarifaController> _logger;

        public TarifaController(ITarifaService tarifaService, ILogger<TarifaController> logger)
        {
            _tarifaService = tarifaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las tarifas activas
        /// </summary>
        /// <returns>Lista de tarifas</returns>
        [HttpGet("GetAllTarifas")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las tarifas");
            var result = await _tarifaService.GetAll();
            
            return result.IsSuccess 
                ? Ok(result) 
                : NotFound(result);
        }

        /// <summary>
        /// Obtiene una tarifa por su ID
        /// </summary>
        /// <param name="id">ID de la tarifa</param>
        /// <returns>Tarifa encontrada</returns>
        [HttpGet("GetTarifaById{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo tarifa por ID: {id}");
            var result = await _tarifaService.GetById(id);
            
            return result.IsSuccess 
                ? Ok(result) 
                : NotFound(result);
        }

        /// <summary>
        /// Obtiene tarifas vigentes para una fecha específica
        /// </summary>
        /// <param name="fecha">Fecha en formato válido (dd/MM/yyyy, yyyy-MM-dd, MM/dd/yyyy, dd-MM-yyyy)</param>
        /// <returns>Lista de tarifas vigentes</returns>
        [HttpGet("GetTarifaByFecha{fecha}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTarifasVigentes(string fecha)
        {
            _logger.LogInformation($"Buscando tarifas vigentes para la fecha: {fecha}");
            var result = await _tarifaService.GetTarifasVigentes(fecha);
            
            if (!result.IsSuccess && result.Message.Contains("formato"))
                return BadRequest(result);
                
            return result.IsSuccess 
                ? Ok(result) 
                : NotFound(result);
        }

        /// <summary>
        /// Obtiene habitaciones por un precio de tarifa específico
        /// </summary>
        /// <param name="precio">Precio de la tarifa</param>
        /// <returns>Lista de habitaciones con el precio especificado</returns>
        [HttpGet("GetTarifaByPrecio/{precio:decimal}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHabitacionesByPrecio(decimal precio)
        {
            if (precio <= 0)
                return BadRequest(new OperationResult { IsSuccess = false, Message = "El precio debe ser mayor que 0." });
                
            _logger.LogInformation($"Buscando habitaciones con precio de tarifa: {precio}");
            var result = await _tarifaService.GetHabitacionesByPrecio(precio);
            
            return result.IsSuccess 
                ? Ok(result) 
                : NotFound(result);
        }

        /// <summary>
        /// Crea una nueva tarifa
        /// </summary>
        /// <param name="dto">Datos de la tarifa a crear</param>
        /// <returns>Tarifa creada</returns>
        [HttpPost("CreateTarifa")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateTarifaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creando nueva tarifa");
            var result = await _tarifaService.Save(dto);
            
            return result.IsSuccess 
                ? Created($"api/tarifa/{result.Data?.GetType().GetProperty("IdTarifa")?.GetValue(result.Data, null)}", result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Actualiza una tarifa existente
        /// </summary>
        /// <param name="dto">Datos de la tarifa a actualizar</param>
        /// <returns>Tarifa actualizada</returns>
        [HttpPut("UpdateTarifaBy{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] UpdateTarifaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Actualizando tarifa con ID: {dto.IdTarifa}");
            var result = await _tarifaService.Update(dto);
            
            return result.IsSuccess 
                ? Ok(result) 
                : result.Message.Contains("No se encontró") 
                    ? NotFound(result) 
                    : BadRequest(result);
        }

        /// <summary>
        /// Elimina una tarifa (cambio de estado)
        /// </summary>
        /// <param name="dto">Datos de la tarifa a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("DeleteTarifaBy{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Eliminando tarifa ID: {Id}", id);
    
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para eliminar tarifa");
                return BadRequest(ModelState);
            }

            var dto = new DeleteTarifaDto() { IdTarifa = id };

            var result = await _tarifaService.Remove(dto);
    
            if (!result.IsSuccess)
                _logger.LogWarning("Error al eliminar la tarifa ID {Id}: {Message}", 
                    dto.IdTarifa, result.Message);
        
            return result.IsSuccess 
                ? Ok(result.Data) 
                : result.Message.Contains("No se encontró") 
                    ? NotFound(new { message = result.Message }) 
                    : BadRequest(new { message = result.Message });
        }
    }
}