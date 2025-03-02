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
                var updatedUserRole = await _userRoleRepository.UpdateEntityAsync(userRole);
                return Ok(updatedUserRole);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error actualizando el rol de usuario {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("descripcion")]
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
        // DELETE api/<UserRoleController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
