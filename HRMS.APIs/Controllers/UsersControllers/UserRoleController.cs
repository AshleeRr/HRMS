using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HRMS.APIs.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserRoleController> _logger;
        public UserRoleController(IUserRoleRepository userRoleRepository, IUserRepository userRepository, ILogger<UserRoleController> logger)
        {
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetAllUserRoles()
        {
            try
            {
                var userRoles = await _userRoleRepository.GetAllAsync();
                if (userRoles == null || !userRoles.Any())
                {
                    return NotFound("No hay roles de usuario guardados");
                }
                return Ok(userRoles);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error obteniendo los roles de usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                var existingUserRole = await _userRoleRepository.GetEntityByIdAsync(id);
                if (existingUserRole == null)
                {
                    return NotFound($"Rol de usuario con id: {id} no encontrado");
                }
                return Ok(existingUserRole);
            } catch (Exception e)
            {
                _logger.LogError(e, $"Error obteniendo rol de usuario con id: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("SaveUserRole")]
        public async Task<IActionResult> SaveUserRole([FromBody] UserRole userRole)
        {
            try
            {
                var createdUserRole = await _userRoleRepository.SaveEntityAsync(userRole);
                return Ok(createdUserRole);
            } catch (Exception e)
            {
                _logger.LogError(e, "Error creando rol de usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")] // actualiza un rol usando su id
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UserRole userRole)
        {
            try
            {
                var existingUserRole = await _userRoleRepository.GetEntityByIdAsync(id);
                if (existingUserRole == null)
                {
                    return NotFound("Rol de usuario no encontrado");
                }
                var updatedUserRole = await _userRoleRepository.UpdateEntityAsync(existingUserRole);
                return Ok(updatedUserRole);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error actualizando el rol de usuario {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("ByDescripcion/{descripcion}")]
        public async Task<IActionResult> GetRoleByDescription(string descripcion)
        {
            try
            {
                if (string.IsNullOrEmpty(descripcion))
                {
                    return BadRequest("La descripcion no puede estar vacia. Asegurese de escribirla correctamente");
                }
                var userRoleDescription = await _userRoleRepository.GetRoleByDescriptionAsync(descripcion);
                if (userRoleDescription == null)
                {
                    return BadRequest("No se ha podido encontrar un usuario con esta descripcion");
                }
                return Ok(userRoleDescription);
            } catch (Exception e)
            {
                _logger.LogError(e, "Error encontrando la descripcion");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        [HttpPatch("UpdateUserRolToUser")]
        public async Task<IActionResult> AsignUserRolToUser(int idUsuario, int idRolUsuario)
        {
            try
            {
                var existingUser = await _userRepository.GetEntityByIdAsync(idUsuario);
                if(existingUser == null)
                {
                    return NotFound($"No se ha encontrado un usuario con id: {idUsuario}");
                }
                var existingUserRole = await _userRoleRepository.GetEntityByIdAsync(idRolUsuario);
                if(existingUserRole == null)
                {
                    return NotFound($"No se ha encontrado un rol con id: {idRolUsuario}");
                }
                var assignedRol = await _userRoleRepository.AsignRolUserAsync(idUsuario, idRolUsuario);
                return Ok(assignedRol);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error actualizando el rol del usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPatch("UpdateDescription/{id}")]
        public async Task<IActionResult> UpdateRoleDescription (int idRolUsuario, string description)
        {
            try
            {
                if(idRolUsuario <1 || string.IsNullOrEmpty(description))
                {
                    return BadRequest("El id debe ser mayor que 0. La descripcion no puede estar vacia");
                }
                var existingRoleUser = await _userRoleRepository.GetEntityByIdAsync(idRolUsuario);
                if(existingRoleUser == null)
                {
                    return BadRequest($"No se ha encontrado un rol con este id: {idRolUsuario}");
                }
                var result = await _userRoleRepository.UpdateDescriptionAsync(idRolUsuario, description);
                return Ok(result);

            } catch (Exception e)
            {
                _logger.LogError(e, "Error editando la descripcion");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        // DELETE api/<UserRoleController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
