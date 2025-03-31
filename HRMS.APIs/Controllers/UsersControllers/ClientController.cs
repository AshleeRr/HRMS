using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet("clients")]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientRepository.GetAllAsync();
            if(clients == null || !clients.Any())
            {
                return NotFound("No hay cLientes guardados");
            }
            return Ok(clients);
        }

        [HttpGet("client/{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            if(id > 0)
            {
                var existingClient = await _clientRepository.GetEntityByIdAsync(id);
                if(existingClient == null)
                {
                    return NotFound($"Cliente con id: {id} no encontrado");
                }
                return Ok(existingClient);
            }
            else
            {
                return BadRequest("El id debe ser mayor que 0");
            }
        }

        [HttpGet("client/email")]
        public async Task<IActionResult> GetClientByEmail(string email) 
        {
            ValidateNull(email, "email");
            var clientEmail = await _clientRepository.GetClientByEmailAsync(email);
            if(clientEmail == null)
            {
                return NotFound($"No se ha encontrado un cliente con este email: {email}");
            }
            return Ok(clientEmail);
        }

        [HttpGet("client/document")]
        public async Task<IActionResult> GetClientByDocument(string document)
        {
            ValidateNull(document, "documento");
            var cliente = await _clientRepository.GetClientByDocumentAsync(document);
            if (cliente == null)
            {
                return NotFound($"No se encontro un cliente con este documento: {document}");
            }
            return Ok(cliente);
        }

        [HttpGet("client/document-type")]
        public async Task<IActionResult> GetClientsByDocumentType(string tipoDocumento)
        {
            ValidateNull(tipoDocumento, "tipo documento");
            var clientes = await _clientRepository.GetClientsByTypeDocumentAsync(tipoDocumento);
            if(!clientes.Any())
            {
               return NotFound("No se han encontrado clientes con este tipo de documento");
            }
            return Ok(clientes);
        }
        private IActionResult ValidateNull(string x, string comment)
        {
            if (string.IsNullOrEmpty(x))
            {
                return BadRequest($"El campo {comment}, no puede estar vacio. Asegurese de escribirlo correctamente");
            }
            return Ok();
        }
    }
}
