using HRMS.Application.DTOs.UsersDTOs.UserRoleDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;
using System.Data;

namespace HRMS.Application.Services.UsersServices
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ILoggingServices _loggerServices;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IValidator<SaveUserRoleDTO> _validator;
        private readonly IUserRepository _userRepository;
        public UserRoleService(IUserRoleRepository userRoleRepository, IValidator<SaveUserRoleDTO> validator,
                                ILoggingServices loggerServices, IUserRepository userRepository)
        {
            _userRoleRepository = userRoleRepository;
            _validator = validator;
            _loggerServices = loggerServices;
            _userRepository = userRepository;
        }
        // mappers
        private UserRole MapSaveDto(SaveUserRoleDTO dto)
        {
            return new UserRole()
            {
                RolNombre = dto.RolNombre,
                Descripcion = dto.Descripcion,
               // FechaCreacion = DateTime.Now,
                UserID = dto.UserID
            };
        }
        private UserRoleViewDTO MapUserRoleToViewDto(UserRole userRole)
        {
            return new UserRoleViewDTO
            {
                IdRolUsuario = userRole.IdRolUsuario,
                RolNombre = userRole.RolNombre,
                Descripcion = userRole.Descripcion,
                ChangeTime = userRole.FechaCreacion,
                UserID = userRole.UserID
            };
        }
        // metodos

        public async Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();
            try
            {
                var userRoles = await _userRoleRepository.GetAllAsync();
                if (!userRoles.Any())
                {
                    result.IsSuccess = false;
                    result.Data = new List<UserRoleDTO>();
                    result.Message = "No hay roles de usuario registrados";
                }
                else
                {
                    result.IsSuccess = true;
                    result.Data = userRoles.Select(MapUserRoleToViewDto).ToList();
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
                result.Data = MapUserRoleToViewDto(userRole);
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
                ValidateId(dto.IdRolUsuario);
                await ValidateUserIDAsync(dto.UserID);
                var rolInUse = await _userRoleRepository.GetUsersByUserRoleIdAsync(dto.IdRolUsuario);
                if (rolInUse.Data != null && rolInUse.Data.Count == 0) 
                {
                    result.Message = "No hay usuarios utilizando el rol, se eliminara el rol seleccionado";

                    var userRole = await _userRoleRepository.GetEntityByIdAsync(dto.IdRolUsuario);
                    ValidateUserRole(userRole);
                    userRole.Estado = false;
                    result = await _userRoleRepository.UpdateEntityAsync(userRole);
                    if (result.IsSuccess)
                    {
                        result.Message = "Rol de usuario eliminado correctamente";
                        result.Data = dto;
                    }
                    else
                    {
                        result.Message = "Error eliminando el rol de usuario";
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Este rol está siendo utilizado por usuarios. No se puede eliminar.";
                    return result;
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
        public async Task<OperationResult> Save(SaveUserRoleDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validDTO = _validator.Validate(dto);
                if (!validDTO.IsSuccess)
                {
                    result.Message = "Error validando los campos para guardar";
                    result.Data = dto;
                    return result; 
                }
                await ValidateUserIDAsync(dto.UserID);
                var userRole = MapSaveDto(dto);
                result = await _userRoleRepository.SaveEntityAsync(userRole);
                if (result.IsSuccess)
                {
                    result.Message = "Rol de usuario guardado correctamente";
                    result.Data = dto;
                }
                else
                {
                    result.Message = "Error guardando el rol de usuario";
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
                ValidateId(dto.IdRolUsuario);
                await ValidateUserIDAsync(dto.UserID);
                var userRole = await _userRoleRepository.GetEntityByIdAsync(dto.IdRolUsuario);
                ValidateUserRole(userRole);
                userRole.Descripcion = dto.Descripcion;
                userRole.RolNombre = dto.RolNombre;
                userRole.FechaCreacion = dto.ChangeTime;
                userRole.UserID = dto.UserID;
                result = await _userRoleRepository.UpdateEntityAsync(userRole);
                if (result.IsSuccess)
                {
                    result.Message = "Rol de usuario actualizado correctamente";
                    result.Data = MapUserRoleToViewDto(userRole);
                }
                else
                {
                    result.Message = "Error actualizando el rol de usuario";
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
        public async Task<OperationResult> UpdateDescriptionAsync(int idRolUsuario, string nuevaDescripcion)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(idRolUsuario);
                var userRole = await _userRoleRepository.GetEntityByIdAsync(idRolUsuario);
                ValidateUserRole(userRole);
                ValidateNulleable(nuevaDescripcion, "nueva descripcion");
                userRole.Descripcion = nuevaDescripcion;
                result = await _userRoleRepository.UpdateEntityAsync(userRole);
                if (!result.IsSuccess) 
                {
                    result.Message = "Error actualizando la descripcion del rol de usuario";
                } else
                {
                    result.Message = "Se actualizó la descripción del rol de usuario";
                    result.Data = MapUserRoleToViewDto(userRole);
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
                ValidateUserRole(userRole);
                ValidateNulleable(nuevoNombre, "nuevo nombre");
                userRole.RolNombre = nuevoNombre;
                result = await _userRoleRepository.UpdateEntityAsync(userRole);
                if (!result.IsSuccess)
                {
                    result.Message = "Error actualizando el nombre del rol de usuario";
                }
                else
                {
                    result.Message = "Se actualizó el nombre del rol de usuario";
                    result.Data = MapUserRoleToViewDto(userRole);
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
                throw new ArgumentException("No existe un rol con este id");
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
        private async Task ValidateUserIDAsync(int userID)
        {
            ValidateId(userID);

            bool userExists = await _userRepository.UserIDExistsAsync(userID);
            if (!userExists)
            {
                throw new ArgumentException("El UserID ingresado no está registrado. No se puede proceder con el proceso");
            }
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
