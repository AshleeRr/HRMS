using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PisoController : ApiControllerBase
    {
        private readonly IPisoService _pisoService;

        public PisoController(IPisoService pisoService, ILogger<PisoController> logger) :
            base (logger)
        {
            _pisoService = pisoService;
        }

        /// <summary>
        /// Obtiene todos los pisos
        /// </summary>
        /// <returns>Lista de pisos</returns>
        [HttpGet("GetAllPisos")]
        [ProducesResponseType(typeof(IEnumerable<PisoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los pisos");
            var result = await _pisoService.GetAll();
            return HandleResponse(result);
        }

        /// <summary>
        /// Obtiene un piso por su ID
        /// </summary>
        /// <param name="id">ID del piso</param>
        /// <returns>Piso encontrado</returns>
        [HttpGet("GetPisoById{id}")]
        [ProducesResponseType(typeof(PisoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Obteniendo piso por ID: {id}");
            
            var validation = ValidateId(id);
            if (validation != null) return validation;
            
            var result = await _pisoService.GetById(id);
            return HandleResponse(result);
        }

        /// <summary>
        /// Busca un piso por su descripción
        /// </summary>
        /// <param name="descripcion">Descripción del piso</param>
        /// <returns>Piso encontrado</returns>
        [HttpGet("GetPisoByDescripcion{descripcion}")]
        [ProducesResponseType(typeof(IEnumerable<PisoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByDescripcion(string descripcion)
        {
            _logger.LogInformation("Buscando pisos con descripción: {Descripcion}", descripcion);
            
            var validation = ValidateString(descripcion , "Piso Descripción");
            if (validation != null) return validation;
            
            var result = await _pisoService.GetPisoByDescripcion(descripcion);
            return HandleResponse(result);
        }

        /// <summary>
        /// Crea un nuevo piso
        /// </summary>
        /// <param name="dto">Datos del piso a crear</param>
        /// <returns>Piso creado</returns>
        [HttpPost("CreatePiso")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreatePisoDto dto)
        { 
            _logger.LogInformation("Creando un nuevo piso {Descripcion}", dto.Descripcion);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _pisoService.Save(dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Error al crear la piso: {Message}", result.Message);
                return BadRequest(CreateProblemDetails(result.Message, StatusCodes.Status400BadRequest));
            }

            var pisoDto = (PisoDto)result.Data;
            return CreatedAtAction(nameof(GetById), new { id = pisoDto.IdPiso }, pisoDto);
        }

        /// <summary>
        /// Actualiza un piso existente
        /// </summary>
        /// <param name="dto">Datos del piso a actualizar</param>
        /// <returns>Piso actualizado</returns>
        [HttpPatch("UpdatePiso{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePisoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id != dto.IdPiso)
            {
                return BadRequest(CreateProblemDetails(
                    "El ID en la URL no coincide con el ID en el cuerpo de la solicitud", 
                    StatusCodes.Status400BadRequest));
            }

            var result = await _pisoService.Update(dto);
            return HandleOperationResult(result);
        }

        /// <summary>
        /// Elimina un piso (cambio de estado)
        /// </summary>
        /// <param name="dto">Datos del piso a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("DeletePiso{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Eliminando el piso con el id {id}", id);
            
            
            var validation = ValidateId(id);
            if (validation!= null) return BadRequest(validation);

            var dto = new DeletePisoDto() { IdPiso = id };
            
            var result = await _pisoService.Remove(dto);

            return HandleResponse(result);
        }
    }
}