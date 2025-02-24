using HRMS.Domain.Entities.Audit;
using HRMS.Persistence.Interfaces.IAuditRepository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HRMS.APIs.Controllers.AuditController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly ILogger<AuditoriaController> _logger;
        public AuditoriaController(IAuditoriaRepository auditoriaRepository, ILogger<AuditoriaController> logger)
        {
            _auditoriaRepository = auditoriaRepository;
            _logger = logger;
        }
        // GET: api/<AuditoriaController>
        [HttpGet("GetAudits")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var audits = await _auditoriaRepository.GetAllAsync();
                if(audits == null || !audits.Any())
                {
                    return NotFound("No hay auditorias registradas");
                }
                return Ok(audits);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error obteniendo las auditorias");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET api/<AuditoriaController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuditById(int id)
        {
            try
            {
                var existingAudit = await _auditoriaRepository.GetEntityByIdAsync(id);
                if(existingAudit == null)
                {
                    return NotFound($"Auditoria con id: {id} no encontrada");
                }
                return Ok(existingAudit);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error obteniendo la auditoria con id: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        [HttpGet("ByUser/{idUsuario}")]
        public async Task<IActionResult> GetAuditByUserId(int idUsuario)
        {
            try
            {
                var audit = await _auditoriaRepository.GetAuditByUserIdAsync(idUsuario);
                if(audit == null || !audit.Any())
                {
                    return NotFound($"No hay alguna auditoria hecha por el usuario: {idUsuario}");
                }
                return Ok(audit);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error obteniendo la auditoria con el id del usuario: {idUsuario}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("ByDateTime")]
        public async Task<IActionResult> GetAuditByDateTime(DateTime dateTime)
        {
            if (dateTime == default)
            {
                return BadRequest("La fecha proporcionada no es válida. Intentlo nuevamente");
            }
            var audit = await _auditoriaRepository.GetAuditByDateTime(dateTime);
            if(audit == null || !audit.Any())
            {
                return NotFound("No hay auditorias hechas en esta fecha");
            }
            return Ok(audit);
        }
        [HttpGet("RealizarAccion")]
        public async Task<IActionResult> RealizarAccion()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null)
            {
                return Unauthorized("Usuario no autenticado");
            }
            var audit = await _auditoriaRepository.LogAuditAsync("El usuario realizo una accion", userId.Value);
            if (!audit.IsSuccess) 
            {
                return NotFound("No hay auditorias hechas en esta fecha");
            }
            return Ok("Auditoría registrada correctamente.");
        }


        // DELETE api/<AuditoriaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
