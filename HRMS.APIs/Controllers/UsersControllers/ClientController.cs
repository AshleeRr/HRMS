using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HRMS.APIs.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientRepository clientRepository, ILogger<ClientController> logger)
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        [HttpGet("GetClients")]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var clients = await _clientRepository.GetAllAsync();
                if(clients == null || !clients.Any())
                {
                    return NotFound("No hay cLientes guardados");
                }
                return Ok(clients);
            } catch(Exception e)
            {
                _logger.LogError(e, "Error obteniendo los clientes");
                return StatusCode(500, "Error interno del servidor");
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            try
            {
                var existingClient = await _clientRepository.GetEntityByIdAsync(id);
                if(existingClient == null)
                {
                    return NotFound($"Cliente con id: {id} no encontrado");
                }
                return Ok(existingClient);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error obteniendo cliente con id: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("SaveClient")]
        public async Task<IActionResult> SaveCLient([FromBody] Client client)
        {
            try
            {
                var createdClient = await _clientRepository.SaveEntityAsync(client);
                return Ok(client);
            }catch(Exception e)
            {
                _logger.LogError(e, "Error guardando cliente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            try
            {
                var existingClient = await _clientRepository.GetEntityByIdAsync(id);
                if (existingClient == null)
                {
                    return NotFound("Cliente no encontrado");
                }
                var updatedClient = await _clientRepository.UpdateEntityAsync(client);
                return Ok(updatedClient);
            } catch(Exception e)
            {
                _logger.LogError(e, "Error actualizando el cliente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("ByEmail")]
        public async Task<IActionResult> GetClientByEmail(string email) 
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("El email no puede estar vacio. Asegurese de escribirlo correctaente");
                }
                var clientEmail = await _clientRepository.GetClientByEmailAsync(email);
                if (clientEmail == null)
                {
                    return NotFound($"No se ha encontrado un cliente con este email: {email}");
                }
                return Ok(clientEmail);
            } catch(Exception e)
            {
                _logger.LogError(e, "Error obteniendo el cliente por su email");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("ByDocument")]
        public async Task<IActionResult> GetClientByDocument(string document)
        {
            try
            {
                if (string.IsNullOrEmpty(document))
                {
                    return BadRequest("El documento no puede ser nulo");
                }
                var cliente = await _clientRepository.GetClientByDocumentAsync(document);
                if (cliente == null)
                {
                    return NotFound($"No se encontro un cliente con este documento: {document}");
                }
                return Ok(cliente);
                
            } catch(Exception e)
            {
                _logger.LogError(e, "Error obteniendo el cliente por su documento");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("ByDocumentType")]
        public async Task<IActionResult> GetClientByDocumentType(string tipoDocumento)
        {
            try
            {
                if (string.IsNullOrEmpty(tipoDocumento))
                {
                    return BadRequest("El tipo de documento no puede estar vacio");
                }
                var clientes = await _clientRepository.GetClientsByTypeDocumentAsync(tipoDocumento);
                if(clientes == null || !clientes.Any())
                {
                    return NotFound("No se han encontrado clientes con este tipo de documento");
                }
                return Ok(clientes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error obteniendo el cliente por su documento");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        // DELETE api/<ClientController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
