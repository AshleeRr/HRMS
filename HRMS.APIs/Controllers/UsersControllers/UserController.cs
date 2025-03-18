using HRMS.Application.DTOs.ClientDTOs;
using HRMS.Application.DTOs.UserDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HRMS.APIs.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IClientService _clientService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, IUserService userService,
                              IClientService clientService,   
                              ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _userService = userService;
            _clientService = clientService;
            _logger = logger;
        }

        // POST api/<UserController>
        [HttpPost("/user")]
        public async Task<IActionResult> SaveUser([FromBody] SaveUserClientDTO user)
        {
            var u = await _userService.Save(user);
            if (!u.IsSuccess)
            {
                return BadRequest("Error al crear un usuario");
                
            }
            _logger.LogInformation("Usuario creado correctamente");
            var createdUser = (User)u.Data;
            var userId = createdUser.IdUsuario;
            
            if (user.IdUserRole == 1)
            {
                var clientDto = new SaveClientDTO
                {
                    NombreCompleto = user.NombreCompleto,
                    Correo = user.Correo,
                    Clave = user.Clave,
                    Documento = user.Documento,
                    TipoDocumento = user.TipoDocumento,
                    IdUserRole = user.IdUserRole,
                    IdUsuario = userId 
                };
                var client = await _clientService.Save(clientDto);
                if (!client.IsSuccess)
                {
                    return BadRequest("Error al crear un cliente");
                }
                return Ok(client);
            }
            return Ok(u);
        }

        // GET: api/<UserController>
        [HttpGet("/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var usuarios = await _userService.GetAll();
            if (!usuarios.IsSuccess)
            {
                return BadRequest("Error obteniendo todos los usuarios");
            }
            return Ok(usuarios);
            
        }

        // GET api/<UserController>/5
        [HttpGet("/user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            ValidateId(id);
            var usuario = await _userService.GetById(id);
            if(usuario == null)
            {
                return BadRequest($"No se ha encontrado ningun usuario con este id: {id}");
            }
            return Ok(usuario);
        }

        [HttpGet("/user/complete-name")]
        public async Task<IActionResult> GetUsersByName(string nombreCompleto)
        {
            ValidateNull(nombreCompleto, "nombre completo");
            var usuario = await _userRepository.GetUsersByNameAsync(nombreCompleto);
            if(usuario == null)
            {
                return BadRequest($"No se han encontrado usuarios con este nombre: {nombreCompleto}");
            }
            return Ok(usuario);
        }

        [HttpGet("/user/email")]
        public async Task<IActionResult> GetUserByEmailAsync(string correo)
        {
            ValidateNull(correo, "correo");
            var usuario = await _userRepository.GetUserByEmailAsync(correo);
            if (usuario == null)
            {  
               return BadRequest($"No se ha encontrado un usuario con este correo: {correo}");
            }
            return Ok(usuario);
            
        }

        [HttpGet("/user/document")]
        public async Task<IActionResult> GetUserByDocumentAsync(string documento)
        {
            ValidateNull(documento, "documento");
            var usuario = await _userRepository.GetUserByDocumentAsync(documento);
            if (usuario == null)
            {
                return BadRequest($"No se ha encontrado un usuario con este documento: {documento}");
            }
            return Ok(usuario);
        }

        [HttpGet("/user/type-document")]
        public async Task<IActionResult> GetUsersByTypeDocumentAsync(string tipoDocumento)
        {
            ValidateNull(tipoDocumento, "tipo documento");
            var usuario = await _userRepository.GetUsersByTypeDocumentAsync(tipoDocumento);
            if (usuario == null)
            {
                return BadRequest($"No se ha encontrado un usuario con este tipo de documento: {tipoDocumento}");
            }
            return Ok(usuario);
        }

        // PUT api/<UserController>/5
        [HttpPut("/user/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserClientDTO user)
        {
            ValidateId(id);
            var existentUser = await _userService.GetById(id);
            if (!existentUser.IsSuccess)
            {
                return BadRequest($"No se ha podido encontrar un usuario con este id:{id}");
            }
            if(user.IdUserRole == 1)
            {
                var client = await _clientService.GetById(id);
                if (client != null)
                {
                    var updatedClient = await _clientService.Update(user);
                    return Ok(updatedClient);
                }
            }
            var updatedUser = await _userService.Update(user);
            _logger.LogInformation("Usuario actualizado correctamente");
            return Ok(updatedUser);
        }

        [HttpPatch("/user/{id}/nombre-completo")]
        public async Task<IActionResult> UpdateNombreCompletoAsync(int id, string nuevoNombre)
        {
            ValidateId(id);
            ValidateNull(nuevoNombre, "nuevoNombre");
            var user = await _userService.UpdateNombreCompletoAsync(id, nuevoNombre);
            if (!user.IsSuccess)
            {
                return BadRequest("Error actualizando el nombre del usuario");
                
            }
            var createdUser = (User)user.Data;
            var userRole = createdUser.IdRolUsuario;
            if(userRole == 1)
            {
                var c = await _clientService.GetClientByUserIdAsync(id);
                if(c == null)
                {
                    return BadRequest("No se ha encontrado el cliente");
                }
                var client = await _clientService.UpdateNombreCompletoAsync(id, nuevoNombre);
                return Ok(client);
            }
            return Ok(user);
        }

        [HttpPatch("/user/{id}/role")]
        public async Task<IActionResult> UpdateUserRoleToUserAsync(int id, int idUserRole)
        {
            ValidateId(id);
            ValidateId(idUserRole);
            var user = await _userService.UpdateUserRoleToUserAsync(id, idUserRole);
            if (user.IsSuccess)
            {
                return Ok(user);
            }
            return BadRequest("Error actualizando el rol del usuario");
        }

        [HttpPatch("/user/{id}/type-document-and-document")]
        public async Task<IActionResult> UpdateTipoDocumentoAndDocumentoAsync(int id, string tipoDocumento, string documento)
        {
            ValidateId(id);
            ValidateNull(documento, "documento");
            ValidateNull(tipoDocumento, "tipo documento");
            var user = await _userService.UpdateTipoDocumentoAndDocumentoAsync(id, tipoDocumento, documento);
            if (!user.IsSuccess)
            {
                return BadRequest("Error actualizando el documento y el tipo del documento del usuario");
            }

            var createdUser = (User)user.Data;
            var userRole = createdUser.IdRolUsuario;
            if(userRole == 1)
            {
                var c = await _clientService.GetClientByUserIdAsync(id);
                if (c == null)
                {
                    return BadRequest("No se ha encontrado el cliente");
                }
                var client = await _clientService.UpdateTipoDocumentoAndDocumentoAsync(id, tipoDocumento, documento);
                return Ok(client);
            }
            return Ok(user);
        }

        [HttpPatch("/users/{id}/password")]
        public async Task<IActionResult> UpdatePasswordAsync(int id, string nuevaClave)
        {
            ValidateId(id);
            ValidateNull(nuevaClave, "nueva clave");

            var user = await _userService.UpdatePasswordAsync(id, nuevaClave);
            if (user.IsSuccess)
            {
                return BadRequest("Error actualizando la clave del usuario");
            }
            var createdUser = (User)user.Data;
            var userRole = createdUser.IdRolUsuario;
            if(userRole == 1)
            {
                var c = await _clientService.GetClientByUserIdAsync(id);
                if (c == null)
                {
                    return BadRequest("No se ha encontrado el cliente");
                }
                var client = await _clientService.UpdatePasswordAsync(id, nuevaClave);
                return Ok(client);
            }
            return Ok(user);
        }

        [HttpPatch("/users/{id}/email")]
        public async Task<IActionResult> UpdateEmailAsync(int id, string email)
        {
            ValidateId(id);
            ValidateNull(email, "email");
            var user = await _userService.UpdateCorreoAsync(id, email);
            if (!user.IsSuccess)
            {
                return BadRequest("Error actualizando el documento y el tipo del documento del usuario");
            }
            var createdUser = (User)user.Data;
            var userRole = createdUser.IdRolUsuario;
            if(userRole == 1)
            {
                var c = await _clientService.GetById(id);
                if (!c.IsSuccess)
                {
                    return BadRequest("No se ha encontrado el cliente");
                }
                var client = await _clientService.UpdateCorreoAsync(id, email);
                return Ok(client);
            }
            return Ok(user);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("/user/{id}")]
        public async Task<IActionResult> Delete([FromBody] RemoveUserClientDTO user)
        {
            var userDeleted = await _userService.Remove(user);
            if (!userDeleted.IsSuccess)
            {
                return BadRequest("Error al eliminar el usuario");
            }
            var c = await _clientService.GetClientByUserIdAsync(user.Id);
            if (c != null)
            {
                var client = await _clientService.Remove(user);
                return Ok(client);
            }
            _logger.LogInformation("Usuario eliminado correctamente");
            return Ok(userDeleted);
        }

        private IActionResult ValidateId(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El id debe ser mayor que 0");
            }
            return Ok();
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
