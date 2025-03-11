using HRMS.Application.DTOs.UserRoleDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;

namespace HRMS.Application.Services.UsersServices
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ILoggingServices _loggerServices;
        private readonly IUserRoleRepository _userRoleRepository;
        public UserRoleService(IUserRoleRepository userRoleRepository,
                                ILoggingServices loggerServices) 
        {
            _userRoleRepository = userRoleRepository;
            _loggerServices = loggerServices;
        }

        public async Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();
            try
            {
                var userRoles = await _userRoleRepository.GetAllAsync();
                if (!userRoles.Any())
                {
                    result.IsSuccess = false;
                    result.Message = "No hay roles de usuario registrados";
                }
                result.IsSuccess = true;
                result.Data = userRoles;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(id);
                var userRole = await _userRoleRepository.GetEntityByIdAsync(id);
                ValidateUserRole(userRole);
                result.IsSuccess = true;
                result.Data = userRole;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> Remove(RemoveUserRoleDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateUserRoletDto(dto);
                var rolInUse = await _userRoleRepository.GetUsersByUserRoleIdAsync(dto.IdUserRole);
                if (rolInUse != null) 
                {
                    result.IsSuccess = false;
                    result.Message = "Este rol está siendo utilizado por usuarios. No se puede eliminar";
                }
                var userRole = await _userRoleRepository.GetEntityByIdAsync(dto.IdUserRole);
                dto.Deleted = true;
                userRole.Estado = false;
                result = await _userRoleRepository.UpdateEntityAsync(userRole);
                result.IsSuccess = true;
                result.Message = "Rol de usuario eliminado correctamente";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<OperationResult> Save(SaveUserRoleDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateUserRoletDto(dto); 
                var userRole = new UserRole()
                {
                    Descripcion = dto.Descripcion,
                };
                result = await _userRoleRepository.SaveEntityAsync(userRole);
                result.IsSuccess = true;
                result.Message = "Rol de usuario guardado correctamente";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<OperationResult> Update(UpdateUserRoleDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateUserRoletDto(dto);
                ValidateId(dto.IdUserRole);
                var userRole = await _userRoleRepository.GetEntityByIdAsync(dto.IdUserRole);
                ValidateUserRole(userRole);
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<OperationResult> UpdateDescriptionAsync(int idRolUsuario, string nuevaDescripcion)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(idRolUsuario);
                var userRole = await _userRoleRepository.GetEntityByIdAsync(idRolUsuario);
                if(string.IsNullOrEmpty(nuevaDescripcion))
                {
                    result.IsSuccess = false;
                    result.Message = "La nueva descripción no puede estar vacía";
                    return result;
                }
                userRole.Descripcion = nuevaDescripcion;
                result = await _userRoleRepository.UpdateEntityAsync(userRole);
                result.IsSuccess = true;
                result.Message = "Se actualizó la descripción del rol de usuario";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<OperationResult> UpdateNameAsync(int idRolUsuario, string nuevoNombre)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(idRolUsuario);
                var userRole = await _userRoleRepository.GetEntityByIdAsync(idRolUsuario);
                if (string.IsNullOrEmpty(nuevoNombre))
                {
                    result.IsSuccess = false;
                    result.Message = "El nuevo nombre no puede estar vacío";
                    return result;
                }
                userRole.RolNombre = nuevoNombre;
                result = await _userRoleRepository.UpdateEntityAsync(userRole);
                result.IsSuccess = true;
                result.Message = "Se actualizó el nombre del rol de usuario";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        private void ValidateUserRoletDto(object dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("Dto nulo");
            }
        }
        private void ValidateUserRole(UserRole userRole)
        {
            if (userRole == null)
            {
                throw new ArgumentNullException("No existe un rol no con este id");
            }
        }
        private void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("El id debe ser mayor que 0");
            }
        }
    }
}
