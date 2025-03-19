using HRMS.Application.DTOs.UserDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;

namespace HRMS.Application.Services.UsersServices
{
    public class UserService : IUserService
    {
        private readonly ILoggingServices _loggerServices;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<SaveUserClientDTO> _validator;
        public UserService(IUserRepository userRepository, IValidator<SaveUserClientDTO> validator,
                                ILoggingServices loggerServices)
        {
            _userRepository = userRepository;
            _validator = validator;
            _loggerServices = loggerServices;
        }
        public async Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();
            try
            {
                var usuarios = await _userRepository.GetAllAsync();
                if (!usuarios.Any())
                {
                    result.IsSuccess = false;
                    result.Message = "No hay usuarios registrados";
                    return result;
                }
                result.IsSuccess = true;
                result.Data = usuarios;
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
                result.Data = usuario;
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
                ValidateId(dto.Id);
                var user = await _userRepository.GetEntityByIdAsync(dto.Id);
                ValidateUser(user);
                dto.Deleted = true;
                user.Estado = false;
                await _userRepository.UpdateEntityAsync(user);
                result.IsSuccess = true; 
                result.Message = "Usuario eliminado correctamente";
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
        public async Task<OperationResult> Save(SaveUserClientDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validDTO = _validator.Validate(dto);
                if (!validDTO.IsSuccess) 
                {
                    result.Message = "Error validando los datos para guardar";
                    result.IsSuccess = false;
                    return result;
                }
                var existingCorreo = await _userRepository.GetUserByEmailAsync(dto.Correo);
                if (existingCorreo != null)
                {
                    result.Message = "Este correo ya esta registrado";
                    result.IsSuccess = false;
                    return result;
                }
                var existingDocument = await _userRepository.GetUserByDocumentAsync(dto.Documento);
                if (existingDocument != null)
                {
                    result.Message = "Este documento ya esta registrado";
                    result.IsSuccess = false;
                    return result;
                }
                var usuario = new User
                {
                    Correo = dto.Correo,
                    Clave = dto.Clave,
                    NombreCompleto = dto.NombreCompleto,
                    Documento = dto.Documento,
                    TipoDocumento = dto.TipoDocumento,
                    IdRolUsuario = dto.IdUserRole,
                    FechaCreacion = DateTime.Now,
                };
                result = await _userRepository.SaveEntityAsync(usuario);
                if (result.IsSuccess)
                {
                    result.Message = "Usuario guardado correctamente";
                    result.Data = usuario;
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
                var user = await _userRepository.GetEntityByIdAsync(dto.IdUsuario);
                ValidateUser(user);
                var existingCorreo = await _userRepository.GetUserByEmailAsync(dto.Correo);
                if (existingCorreo != null && existingCorreo.IdUsuario != dto.IdUsuario)
                {
                    result.Message = "Este correo ya esta registrado";
                    result.IsSuccess = false;
                }
                var existingDocument = await _userRepository.GetUserByDocumentAsync(dto.Documento);
                if (existingDocument != null && existingDocument.IdUsuario != dto.IdUsuario)
                {
                    result.Message = "Este documento ya esta registrado";
                    result.IsSuccess = false;
                }
                user.Correo = dto.Correo;
                user.NombreCompleto = dto.NombreCompleto;
                user.TipoDocumento = dto.TipoDocumento;
                user.Documento = dto.Documento;
                user.Clave = dto.Clave;
                dto.ChangeTime = DateTime.Now;
                result = await _userRepository.UpdateEntityAsync(user);
                if (result.IsSuccess)
                {
                    result.Message = "Usuario actualizado correctamente";
                    result.IsSuccess = true;
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

                var existingCorreo = await _userRepository.GetUserByEmailAsync(nuevoCorreo);
                if (existingCorreo != null && existingCorreo.IdUsuario != idUsuario)
                {
                    result.Message = "Este correo ya esta registrado";
                    result.IsSuccess = true;
                }
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
                    result.Data = user;

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
                    result.IsSuccess = false;
                    result.Message = "Error actualizando el nombre";
                } else
                {
                    result.Message = "Nombre actualizado correctamente";
                    result.Data = user;

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
                await _userRepository.UpdateEntityAsync(user);
                result.IsSuccess = true;
                result.Message = "Rol de usuario actualizado al usuario correctamente";
                result.Data = user;
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

                var existingDocument = await _userRepository.GetUserByDocumentAsync(documento);
                if (existingDocument != null && existingDocument.IdUsuario != idUsuario)
                {
                    result.Message = "Este documento ya esta registrado";
                    result.IsSuccess = true;
                }
                var usuario = await _userRepository.GetEntityByIdAsync(idUsuario);
                ValidateUser(usuario);
                usuario.TipoDocumento = tipoDocumento;
                usuario.Documento = documento;
                await _userRepository.UpdateEntityAsync(usuario);
                result.IsSuccess = true;
                result.Message = "Datos actualizados correctamente";
                result.Data = usuario;
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
                await _userRepository.UpdateEntityAsync(usuario);
                result.IsSuccess = true;
                result.Message = "Clave actualizada correctamente";
                result.Data = usuario;
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
