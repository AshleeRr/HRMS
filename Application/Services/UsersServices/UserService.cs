using HRMS.Application.DTOs.UsersDTOs.UserDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Infraestructure.Notification;

namespace HRMS.Application.Services.UsersServices
{
    public class UserService : IUserService
    {
        private readonly ILoggingServices _loggerServices;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<SaveUserDTO> _validator;
        private readonly INotificationService _notificationService;
        public UserService(IUserRepository userRepository, IValidator<SaveUserDTO> validator,
                                INotificationService notificationService,
                                ILoggingServices loggerServices)
        {
            _userRepository = userRepository;
            _validator = validator;
            _loggerServices = loggerServices;
            _notificationService = notificationService;
        }
        // mappers
        // mappers
        private User MapSaveDto(SaveUserDTO dto)
        {
            return new User
            {
                IdRolUsuario = dto.IdRolUsuario,
              //  FechaCreacion = DateTime.Now,
                NombreCompleto = dto.NombreCompleto,
                Correo = dto.Correo,
                Clave = dto.Clave,
                TipoDocumento = dto.TipoDocumento,
                Documento = dto.Documento,
                UserID = dto.UserID,
            };
        }
        private UserViewDTO MapUserToViewDTO(User user)
        {
            return new UserViewDTO
            {
                IdUsuario = user.IdUsuario,
                ReferenceID = user.ReferenceID,
                IdRolUsuario = user.IdRolUsuario,
                NombreCompleto = user.NombreCompleto,
                Correo = user.Correo,
                Clave = user.Clave,
                TipoDocumento = user.TipoDocumento,
                Documento = user.Documento,
                ChangeTime = user.FechaCreacion,
                UserID = user.UserID
            };
        }

        //methods
        public async Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();
            try
            {
                var usuarios = await _userRepository.GetAllAsync();
                if (!usuarios.Any())
                {
                    result.IsSuccess = false;
                    result.Data = new List<UserDTO>();
                    result.Message = "No hay usuarios registrados";
                }
                else
                {
                    result.IsSuccess = true;
                    result.Data = usuarios.Select(MapUserToViewDTO).ToList();
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
                var usuario = await _userRepository.GetEntityByIdAsync(id);
                ValidateUser(usuario);
                result.IsSuccess = true;
                result.Data = MapUserToViewDTO(usuario);
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
        public async Task<OperationResult> Remove(RemoveUserClientDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(dto.IdUsuario);
                await ValidateUserIDAsync(dto.UserID);
                var user = await _userRepository.GetEntityByIdAsync(dto.IdUsuario);
                ValidateUser(user);
                if(user.Estado == false)
                {
                    result.IsSuccess = false;
                    result.Message = "El usuario ya habia sido eliminado previamente";
                }
                else
                {
                    user.Estado = false;
                    result = await _userRepository.UpdateEntityAsync(user);
                    if (result.IsSuccess == true)
                    {
                        result.Message = "Usuario eliminado correctamente";
                        result.Data = dto;
                    }
                    else
                    {
                        result.Message = "Error eliminando al usuario";
                    }
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
        public async Task<OperationResult> Save(SaveUserDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validDTO = _validator.Validate(dto);
                if (!validDTO.IsSuccess) 
                {
                    result.Message = "Error validando los datos para guardar";
                    result.Data = dto;
                    return result;
                }
                await ValidateUserIDAsync(dto.UserID);
                var usuario = MapSaveDto(dto);
                result = await _userRepository.SaveEntityAsync(usuario);
                if (result.IsSuccess)
                {
                    result.Message = "Usuario guardado correctamente";
                    result.Data = MapUserToViewDTO(usuario);
                }
                else
                {
                    result.Message = "Error guardando el usuario";
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
        public async Task<OperationResult> Update(UpdateUserClientDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(dto.IdUsuario);
                await ValidateUserIDAsync(dto.UserID);
                var user = await _userRepository.GetEntityByIdAsync(dto.IdUsuario);
                ValidateUser(user);

                user.Correo = dto.Correo;
                user.NombreCompleto = dto.NombreCompleto;
                user.TipoDocumento = dto.TipoDocumento;
                user.Documento = dto.Documento;
                user.Clave = dto.Clave;
                user.FechaCreacion = dto.ChangeTime;
                user.UserID = dto.UserID;
                result = await _userRepository.UpdateEntityAsync(user);
                if (result.IsSuccess)
                {
                    result.Message = "Usuario actualizado correctamente";
                    result.Data = MapUserToViewDTO(user);
                    _notificationService.SendNotification(user.IdUsuario, $"No responda a este mensaje.\nEstimado usuario le informamos que sus datos han sido actualizados.\nCorreo asociado: {user.Correo}.\nNombre: {dto.NombreCompleto}.\nCorreo: {dto.Correo}.\nTipo de documento: {dto.TipoDocumento}.\nDocumento: {dto.Documento}.\n Si no ha realizado esta modificacion o tiene alguna duda consulte con el equipo de administracion.");
                }
                else
                {
                    result.Message = "Error actualizando al usuario";
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
        public async Task<OperationResult> UpdateCorreoAsync(int idUsuario, string nuevoCorreo)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(idUsuario);
                ValidateNulleable(nuevoCorreo, "nuevo correo");
                var user = await _userRepository.GetEntityByIdAsync(idUsuario);
                ValidateUser(user);
                user.Correo = nuevoCorreo;
                result = await _userRepository.UpdateEntityAsync(user);
                if (!result.IsSuccess)
                {
                    result.Message = "Error actualizando el correo";
                }
                else
                {
                    result.Message = "Correo actualizado correctamente";
                    result.Data = MapUserToViewDTO(user);
                    _notificationService.SendNotification(user.IdUsuario, $"No responda a este mensaje.\nSu correo ha sido actualizado con éxito.\nNuevo correo asociado: {nuevoCorreo}. \nSi no ha realizado esta mmodificacion o tiene alguna duda consulte con el equipo de administracion.");
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
        public async Task<OperationResult> UpdateNombreCompletoAsync(int idUsuario, string nuevoNombreCompleto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(idUsuario);
                ValidateNulleable(nuevoNombreCompleto, "nuevo nombre");
                var user = await _userRepository.GetEntityByIdAsync(idUsuario);
                ValidateUser(user);
                user.NombreCompleto = nuevoNombreCompleto;
                result = await _userRepository.UpdateEntityAsync(user);
                if (!result.IsSuccess)
                {
                    result.Message = "Error actualizando el nombre";
                } else
                {
                    result.Message = "Nombre actualizado correctamente";
                    result.Data = MapUserToViewDTO(user);
                    _notificationService.SendNotification(user.IdUsuario, $"No responda a este mensaje.\nSu nombre ha sido actualizado con éxito.\nNuevo nombre asociado: {nuevoNombreCompleto}. \nSi no ha realizado esta modificacion o tiene alguna duda consulte con el equipo de administracion.");
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
        public async Task<OperationResult> UpdateUserRoleToUserAsync(int idUsuario, int idUserRole)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(idUsuario);
                ValidateId(idUserRole);
                var user = await _userRepository.GetEntityByIdAsync(idUsuario);
                ValidateUser(user);
                user.IdRolUsuario = idUserRole;
                result = await _userRepository.UpdateEntityAsync(user);
                if(result.IsSuccess == true)
                {
                    result.Message = "Rol de usuario actualizado al usuario correctamente";
                    result.Data = MapUserToViewDTO(user);
                }
                else
                {
                    result.Message = "Error actualizando el rol de usuario al usuario";
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
        public async Task<OperationResult> UpdateTipoDocumentoAndDocumentoAsync(int idUsuario, string tipoDocumento, string documento)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(idUsuario);
                ValidateNulleable(tipoDocumento, "tipo de documento");
                ValidateNulleable(documento, "documento");
                var usuario = await _userRepository.GetEntityByIdAsync(idUsuario);
                ValidateUser(usuario);
                usuario.TipoDocumento = tipoDocumento;
                usuario.Documento = documento;
                result = await _userRepository.UpdateEntityAsync(usuario);
                if (result.IsSuccess)
                {
                    result.Message = "Datos actualizados correctamente";
                    result.Data = MapUserToViewDTO(usuario);
                    _notificationService.SendNotification(usuario.IdUsuario, $"No responda a este mensaje.\nSu documento de identidad ha sido actualizado con éxito.\nNuevo documento asociado: {documento}.\nTipo de documento: {tipoDocumento} \nSi no ha realizado esta modificacion o tiene alguna duda consulte con el equipo de administracion.");
                }
                else
                {
                    result.Message = "Error actualizando el tipo de documento y el documento";
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
        public async Task<OperationResult> UpdatePasswordAsync(int idUsuario, string nuevaClave)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(idUsuario);
                ValidateClave(nuevaClave);
                var usuario = await _userRepository.GetEntityByIdAsync(idUsuario);
                ValidateUser(usuario);
                usuario.Clave = nuevaClave;
                result = await _userRepository.UpdateEntityAsync(usuario);
                if (result.IsSuccess)
                {
                    result.Message = "Clave actualizada correctamente";
                    result.Data = MapUserToViewDTO(usuario);
                    _notificationService.SendNotification(usuario.IdUsuario, $"No responda a este mensaje.\nClave ha sido actualizado con éxito.\nSi no ha realizado esta modificacion o tiene alguna duda consulte con el equipo de administracion.");
                }
                else
                {
                    result.Message = "Error actualizando la clave";
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
        private void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El id del usuario debe ser mayor que 0");
            }
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
        private void ValidateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("El usuario no existe");
            }
        }
        private void ValidateNulleable(string x, string message)
        {
            if (string.IsNullOrEmpty(x))
            {
                throw new ArgumentNullException($"El campo: {message} no puede estar vacio.");
            }
        }
        
        private void ValidateClave(string? clave)
        {
            if (string.IsNullOrEmpty(clave))
            {
                throw new ArgumentException("La clave no puede estar vacia");
            }

            if (clave.Length < 12 || clave.Length > 50)
            {
                throw new ArgumentException("La clave debe contener tener entre 12 y 50 caracteres");
            }

            if (!clave.Any(char.IsUpper) || !clave.Any(char.IsDigit) || !clave.Any(char.IsLower))
            {
                throw new ArgumentException("La clave debe contener al menos una letra mayuscula, una minuscula y un numero");
            }

            string caracteresEspeciales = "@#!*?$/,{}=.;:";
            if (!clave.Any(c => caracteresEspeciales.Contains(c)))
            {
                throw new ArgumentException("La clave debe contener al menos un carácter especial.");
            }

            if (clave.Contains(" "))
            {
                throw new ArgumentException("La clave no debe contener espacios.");
            }
        }
    }
}
