using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarifaController : ApiControllerBase
    {
        private readonly ITarifaService _tarifaService;

        public TarifaController(ITarifaService tarifaService, ILogger<TarifaController> logger)
        : base (logger)
        {
            _tarifaService = tarifaService;
        }

        /// <summary>
        /// Obtiene todas las tarifas activas
        /// </summary>
        /// <returns>Lista de tarifas</returns>
        [HttpGet("GetAllTarifas")]
        [ProducesResponseType(typeof(IEnumerable<TarifaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las tarifas");
            var result = await _tarifaService.GetAll();

            return HandleResponse(result);
        }

        /// <summary>
        /// Obtiene una tarifa por su ID
        /// </summary>
        /// <param name="id">ID de la tarifa</param>
        /// <returns>Tarifa encontrada</returns>
        [HttpGet("GetTarifaById{id}")]
        [ProducesResponseType(typeof(TarifaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo tarifa por ID: {id}");
            var validation = ValidateId(id);
            if (validation != null) return validation;
            
            var result = await _tarifaService.GetById(id);
            return HandleResponse(result);
        }

        /// <summary>
        /// Obtiene tarifas vigentes para una fecha específica
        /// </summary>
        /// <param name="fecha">Fecha en formato válido (dd/MM/yyyy, yyyy-MM-dd, MM/dd/yyyy, dd-MM-yyyy)</param>
        /// <returns>Lista de tarifas vigentes</returns>
        [HttpGet("GetTarifaByFecha{fecha}")]
        [ProducesResponseType(typeof(IEnumerable<TarifaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTarifasVigentes(string fecha)
        {
            _logger.LogInformation($"Buscando tarifas vigentes para la fecha: {fecha}");
            var result = await _tarifaService.GetTarifasVigentes(fecha);
            return HandleResponse(result);
        }

        /// <summary>
        /// Obtiene habitaciones por un precio de tarifa específico
        /// </summary>
        /// <param name="precio">Precio de la tarifa</param>
        /// <returns>Lista de habitaciones con el precio especificado</returns>
        [HttpGet("GetTarifaByPrecio/{precio:decimal}")]
        [ProducesResponseType(typeof(IEnumerable<TarifaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHabitacionesByPrecio(decimal precio)
        {            
            _logger.LogInformation($"Buscando habitaciones con precio de tarifa: {precio}");

            if (precio <= 0)
            {
                return BadRequest("El precio debe ser mayor a 0");
            }
                
            var result = await _tarifaService.GetHabitacionesByPrecio(precio);
            
            return HandleResponse(result);
        }

        /// <summary>
        /// Crea una nueva tarifa
        /// </summary>
        /// <param name="dto">Datos de la tarifa a crear</param>
        /// <returns>Tarifa creada</returns>
        [HttpPost("CreateTarifa")]
        [ProducesResponseType(typeof(TarifaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateTarifaDto dto)
        {
         
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _tarifaService.Save(dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Error al crear la tarifa: {Message}", result.Message);
                return BadRequest(CreateProblemDetails(result.Message, StatusCodes.Status400BadRequest));
            }

            var tarifaDto = (TarifaDto)result.Data;
            return CreatedAtAction(nameof(GetById), new { id = tarifaDto.IdTarifa }, tarifaDto);
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTarifaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id != dto.IdTarifa)
            {
                return BadRequest(CreateProblemDetails(
                    "El ID en la URL no coincide con el ID en el cuerpo de la solicitud", 
                    StatusCodes.Status400BadRequest));
            }

            var result = await _tarifaService.Update(dto);
            return HandleOperationResult(result);
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
            
            var validation = ValidateId(id);
            if (validation!= null) return BadRequest(validation);

            var dto = new DeleteTarifaDto() { IdTarifa = id };
            
            var result = await _tarifaService.Remove(dto);

            return HandleResponse(result);
        }
    }
}