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
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }


        // GET: api/<UserController>
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                if (users == null || !users.Any())
                {
                    return NotFound("No hay usuarios guardados");
                }
                 return Ok(users);
            } catch(Exception e)
            {
                _logger.LogError(e, "Error obteniendo los usuarios");
                return StatusCode(500, "Error interno del servidor");
            }
            
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsersById(int id)
        {
            try
            {
                var users = await _userRepository.GetEntityByIdAsync(id);
                if (users == null)
                {
                    return NotFound($"No hay un usuario con este id: {id}");
                }
                return Ok(users);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error obteniendo usuario por id");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // POST api/<UserController>
        [HttpPost("SaveUser")]
        public async Task<IActionResult> SaveUser ([FromBody] User user)
        {
            try
            {
                var createdUser = await _userRepository.SaveEntityAsync(user);
                return Ok(createdUser);

            } catch(Exception e)
            {
                _logger.LogError(e, "Error guardando usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                var existingUser = await _userRepository.GetEntityByIdAsync(id);
                if(existingUser == null)
                {
                    return NotFound($"No se ha encontrado un usuario con id: {id}");
                }
                var uptadedUser = await _userRepository.UpdateEntityAsync(user);
                return Ok(uptadedUser);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error actualizando el usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        [HttpPatch("UpdateEstado")]
        public async Task<IActionResult> UpdateEstado([FromBody] User user, bool nuevoEstado)
        {
            try
            {
                var usuario = await _userRepository.UpdateEstadoAsync(user, nuevoEstado);
                return Ok(usuario);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error actualizando el estado del usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        [HttpGet("ByCompleteName/{nombreCompleto}")]
        public async Task<IActionResult> GetUserByName(string nombreCompleto)
        {
            try
            {
                if (string.IsNullOrEmpty(nombreCompleto))
                {
                    return BadRequest("El nombre no puede estar vacio. Asegurese de escribirlo correctamente ");
                }
                var users = await _userRepository.GetUsersByNameAsync(nombreCompleto);
                if(users == null || !users.Any())
                {
                    return NotFound($"No se han encontrado usuarios con el nombre: {nombreCompleto}");
                }
                return Ok(users);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error obteniendo usuarios con el nombre: {nombreCompleto}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPatch("UdateUserPassword")]
        public async Task<IActionResult> UpdateUserPassword (int idUsuario, string clave)
        {
            try
            {
                if (string.IsNullOrEmpty(clave))
                {
                    return BadRequest("La clave no puede estar vacia. Asegurese de escribirla correctamente ");
                }
                var updatedPassword = await _userRepository.UpdatePasswordAsync(idUsuario, clave);
                return Ok(updatedPassword);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error actualizando la clave del usuario con el id: {idUsuario}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("GetUsersByRole/{idUserRole}")]
        public async Task<IActionResult> GetUsersByUserRole(int idUserRole)
        {
            try
            {
                var userByRole = await _userRepository.GetUsersByUserRoleIdAsync(idUserRole);
                return Ok(userByRole);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error obteniendo los usuarios");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
