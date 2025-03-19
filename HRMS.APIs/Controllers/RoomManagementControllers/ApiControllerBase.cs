using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected readonly ILogger<ApiControllerBase> _logger;

        public ApiControllerBase(ILogger<ApiControllerBase> logger)
        {
            _logger = logger;
        }

        protected ProblemDetails CreateProblemDetails(string detail, int statusCode)
        {
            return new ProblemDetails()
            {
                Detail = detail,
                Status = statusCode,
                Title = statusCode == StatusCodes.Status400BadRequest
                    ? "Recurso no encontrado"
                    : "Error de solicitud"
            };
        }

        protected IActionResult HandleResponse (OperationResult result, bool returnDetail = true)
        {
            if (!result.IsSuccess)
            {
                _logger.LogError($"Operacion  fallida: {result.Message}");
                
                return result.Message?.Contains("No se encontraron") == true
                    ? NotFound(CreateProblemDetails(result.Message, StatusCodes.Status404NotFound))
                    : BadRequest(CreateProblemDetails(result.Message ?? "Error dessconocido", 
                        StatusCodes.Status400BadRequest));
            }
            return Ok(returnDetail ? result.Data : result);
        }

        protected IActionResult HandleOperationResult(OperationResult result)
        {
            return HandleResponse(result, false);
        }
        
        protected IActionResult ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Solicitud con ID inválido: {Id}", id);
                return BadRequest(new { message = "El ID debe ser mayor que cero." });
            }
            return null;
        }
        
        protected IActionResult ValidateString( string value , string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogWarning("Solicitud con {FieldName} inválido: {Value}", fieldName, value);
                return BadRequest(new { message = $"El campo {fieldName} no puede estar vacío." });
            }
            return null;
        }
    }
}
