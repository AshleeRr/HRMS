using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.AspNetCore.Mvc;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Application.DTOs.UsersDTOs.UserRoleDTOs;
using Newtonsoft.Json;

namespace HRMS.APIs.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRoleService _userRoleService;
        private readonly ILogger<UserRoleController> _logger;
        public UserRoleController(IUserRoleRepository userRoleRepository,
                                  IUserRoleService userRoleService,
                                  ILogger<UserRoleController> logger)
        {
            _userRoleRepository = userRoleRepository;
            _logger = logger;
            _userRoleService = userRoleService;
        }

        [HttpPost("role")]
        public async Task<IActionResult> SaveUserRole([FromBody] SaveUserRoleDTO userRole)
        {
            var createdUserRole = await _userRoleService.Save(userRole);
            if (createdUserRole.IsSuccess)
            {
                _logger.LogInformation("Rol de usuario creado correctamente");
                return Ok(createdUserRole);
            }

            return BadRequest("Error al crear un nuevo rol");
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllUserRoles()
        {
            var userRoles = await _userRoleService.GetAll();
            if (!userRoles.IsSuccess)
            {
                return BadRequest("Error obteniendo todos los roles de usuario");
            }
            return Ok(userRoles);
        }

        [HttpGet("role/{id}")]
        public async Task<IActionResult> GetUserRoleById(int id)
        {
            ValidateId(id);
            var existingUserRole = await _userRoleRepository.GetEntityByIdAsync(id);
            if (existingUserRole == null)
            {
                return NotFound($"Rol de usuario con id: {id} no encontrado");
            }
            return Ok(existingUserRole);
        }

        [HttpGet("role/description")]
        public async Task<IActionResult> GetRoleByDescription(string descripcion)
        {
            ValidateNull(descripcion, "descripcion");
            var userRole = await _userRoleRepository.GetRoleByDescriptionAsync(descripcion);
            if (userRole == null)
            {
                return BadRequest("No se ha podido encontrar un rol con esta descripcion");
            }
            return Ok(userRole);
        }

        [HttpGet("role/name")]
        public async Task<IActionResult> GetRoleByName(string nombre)
        {
            ValidateNull(nombre, "nombre");
            var userRole = await _userRoleRepository.GetRoleByNameAsync(nombre);
            if (userRole == null)
            {
                return BadRequest("No se ha podido encontrar un rol con este nombre");
            }
            return Ok(userRole);
        }

        [HttpGet("role/{id}/users")]
        public async Task<IActionResult> GetUserFromARole(int id)
        {
            ValidateId(id);
            var users = await _userRoleRepository.GetUsersByUserRoleIdAsync(id);
            if (!users.IsSuccess)
            {
                return BadRequest("No se ha podido encontrar usuarios con este rol");
            }
            return Ok(users);
        }

        [HttpPut("role/{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDTO userRole)
        {
            ValidateNull(userRole.Descripcion, "descripcion");
            ValidateNull(userRole.Nombre, "nombre");
            ValidateId(id);
            var rol = await _userRoleService.GetById(id);
            if (rol == null)
            {
                return NotFound("Rol de usuario no encontrado");
            }
            var updatedRol = await _userRoleService.Update(userRole);
            if (!updatedRol.IsSuccess)
            {
                return BadRequest("Ocurrio un error actualizando el rol");
            }
            _logger.LogInformation("Rol actualizado correctamente");
            return Ok(updatedRol);
        }

        [HttpPatch("role/{id}/description")]
        public async Task<IActionResult> UpdateDescription(int id, string descripcion)
        {
            ValidateId(id);
            ValidateNull(descripcion, "descripcion");
            var rolNewDesc = await _userRoleService.UpdateDescriptionAsync(id, descripcion);
            if (!rolNewDesc.IsSuccess)
            {
                return BadRequest("Error actualizando la descripcion del rol");
            }
            return Ok(rolNewDesc);
        }

        [HttpPatch("role/{id}/name")]
        public async Task<IActionResult> UpdateName(int id, string nombre)
        {
            ValidateId(id);
            ValidateNull(nombre, "nombre");
            var rolNewName = await _userRoleService.UpdateNameAsync(id, nombre);
            if (!rolNewName.IsSuccess)
            {
                return BadRequest("Error actualizando el nombre del rol");
            }
            return Ok(rolNewName);
        }

        [HttpDelete("role/{id}")]
        public async Task<IActionResult> Delete([FromBody] RemoveUserRoleDTO dto )
        {
            if(dto.IdUserRole > 0){
                var rol = await _userRoleService.Remove(dto);
                if (!rol.IsSuccess)
                {
                    return BadRequest($"Error eliminando el rol: {rol.Message}");
                }
                _logger.LogInformation("Rol eliminado correctamente");
                return Ok(rol);

            }
            else
            {
                return BadRequest("El id debe ser mayor que 0");
            }
            
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
