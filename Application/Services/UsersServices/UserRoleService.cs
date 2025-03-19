using HRMS.Application.DTOs.UserRoleDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;

namespace HRMS.Application.Services.UsersServices
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ILoggingServices _loggerServices;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IValidator<SaveUserRoleDTO> _validator;
        public UserRoleService(IUserRoleRepository userRoleRepository, IValidator<SaveUserRoleDTO> validator,
                                ILoggingServices loggerServices) 
        {
            _userRoleRepository = userRoleRepository;
            _validator = validator;
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
                else
                {
                    result.IsSuccess = true;
                    result.Data = userRoles;
                }
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
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
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
                ValidateId(dto.IdUserRole);
                var rolInUse = await _userRoleRepository.GetUsersByUserRoleIdAsync(dto.IdUserRole);
                if (!rolInUse.IsSuccess || rolInUse.Data.Any()) 
                {
                    result.IsSuccess = false;
                    result.Message = "Este rol está siendo utilizado por usuarios. No se puede eliminar";
                    return result;
                }
                var userRole = await _userRoleRepository.GetEntityByIdAsync(dto.IdUserRole);
                dto.Deleted = true;
                userRole.Estado = false;
                result = await _userRoleRepository.UpdateEntityAsync(userRole);
                result.IsSuccess = true;
                result.Message = "Rol de usuario eliminado correctamente";
                result.Data = dto;
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
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
                var validDTO = _validator.Validate(dto);
                if (!validDTO.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "Error validando los campos para guardar";
                    result.Data = dto;
                }
                var userRole = new UserRole()
                {
                    Descripcion = dto.Descripcion,
                    RolNombre = dto.Nombre,
                };
                result = await _userRoleRepository.SaveEntityAsync(userRole);
                if (result.IsSuccess)
                {
                    result.IsSuccess = true;
                    result.Message = "Rol de usuario guardado correctamente";
                    result.Data = dto;
                }
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
                ValidateId(dto.IdUserRole);
                var userRole = await _userRoleRepository.GetEntityByIdAsync(dto.IdUserRole);
                ValidateUserRole(userRole);
                userRole.Descripcion = dto.Descripcion;
                userRole.RolNombre = dto.Nombre;
                dto.ChangeTime = DateTime.Now;
                await _userRoleRepository.UpdateEntityAsync(userRole);
                result.Message = "Rol de usuario actualizado correctamente";
                result.IsSuccess = true;
                result.Data = dto;
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
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
                ValidateNulleable(nuevaDescripcion, "nueva descripcion");
                userRole.Descripcion = nuevaDescripcion;
                result = await _userRoleRepository.UpdateEntityAsync(userRole);
                if (!result.IsSuccess) 
                {
                    result.IsSuccess = false;
                    result.Message = "Error actualizando la descripcion del rol de usuario";
                } else
                {
                    result.Message = "Se actualizó la descripción del rol de usuario";
                }
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
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
                ValidateNulleable(nuevoNombre, "nuevo nombre");
                userRole.RolNombre = nuevoNombre;
                result = await _userRoleRepository.UpdateEntityAsync(userRole);
                if (!result.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "Error actualizando el nombre del rol de usuario";
                }
                else
                {
                    result.Message = "Se actualizó el nombre del rol de usuario";
                }
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        private UserRole ValidateUserRole(UserRole userRole)
        {
            if (userRole == null)
            {
                throw new ArgumentNullException("No existe un rol con este id");
            }
            return userRole;
        }
        private int ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El id debe ser mayor que 0");
            }
            return id;
        }
        private void ValidateNulleable(string x, string message)
        {
            if (string.IsNullOrEmpty(x))
            {
                throw new ArgumentException($"El campo: {message} no puede estar vacio.");
            }
        }
    }
}
