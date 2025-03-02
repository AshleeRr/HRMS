using HRMS.Application.DTOs.UserRoleDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.UsersServices
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRoleService> _logger;
        private readonly IUserRoleRepository _userRoleRepository;
        public UserRoleService(IUserRoleRepository userRoleRepository,
                                ILogger<UserRoleService> logger, IConfiguration configuration) 
        {
            _userRoleRepository = userRoleRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public Task<OperationResult> AsignDefaultRoleAsync(int idUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> AsignRolUserAsync(int idUsuario, int idRolUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Remove(RemoveUserRoleDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Save(SaveUserRoleDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Update(UpdateUserRoleDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> UpdateDescriptionAsync(int idRolUsuario, string nuevaDescripcion)
        {
            throw new NotImplementedException();
        }




        /*
        public async Task<OperationResult> AsignDefaultRoleAsync(int idUsuario)
        { // asigna un rol por defecto
            OperationResult result = new OperationResult();
            try
            {
               const int rolPredeterminado = 1; // el rol predeterminado debe ser cliente
               var usuario = await _context.Users.FindAsync(idUsuario);
                if(usuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un usuario con ese id";
                    return result;
                }
                usuario.IdRolUsuario = rolPredeterminado;
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Rol predeterminado asignado";

            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRolRepository: AsignDefaultRoleAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }*/
        /*
        public async Task<OperationResult> UpdateDescriptionAsync(int idRolUsuario, string nuevaDescripcion)
        {
            OperationResult result = new OperationResult();
            try
            {

                if (!Validation.ValidateId(idRolUsuario, result))
                    return result;

                if(!Validation.ValidateDescription(nuevaDescripcion, result))
                    return result;
                var rolUsuario = await _context.UserRoles.FindAsync(idRolUsuario);
                if (rolUsuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un rol de usuario con ese id";
                    return result;
                }
                rolUsuario.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Se actualizó la descripción del rol de usuario";
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRolRepository: UpdateDescriptionAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }*/

        /*
        public async Task<OperationResult> AsignRolUserAsync(int idUsuario, int idRolUsuario)
        { // asigna un rol a un usuario
            OperationResult result = new OperationResult();
            try
            {
                if (!Validation.ValidateId(idUsuario, result))
                    return result;
                if (!Validation.ValidateId(idRolUsuario, result))
                    return result;
                var usuario = await _context.Users.FindAsync(idUsuario);
                if (usuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un usuario con ese id";
                    return result;
                }
                var userRol = await _context.UserRoles.FindAsync(idRolUsuario);
                if (userRol == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un rol de usuario con ese id";
                    return result;
                }
                usuario.IdRolUsuario = idRolUsuario;
                _context.Users.Update(usuario);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Rol asignado correctamente";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = _configuration["ErrorUserRepository: AsignRolUserAsync"];
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }*/
    }
}
