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
                result.Message = "Usuario eliminado";
                result.Data = dto;
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
                    result.IsSuccess = true;
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
                    result.Message = "Usuario guardado";
                    result.IsSuccess = true;
                    result.Data = usuario;
                }
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
                user.Correo = dto.Correo;
                user.NombreCompleto = dto.NombreCompleto;
                user.TipoDocumento = dto.TipoDocumento;
                user.Documento = dto.Documento;
                user.Clave = dto.Clave;
                dto.ChangeTime = DateTime.Now;
                await _userRepository.UpdateEntityAsync(user);
                result.Message = "Usuario actualizado";
                result.IsSuccess = true;
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
                await _userRepository.UpdateEntityAsync(user);
                result.Message = "Correo actualizado";
                result.IsSuccess = true;
                result.Data = user;
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
                await _userRepository.UpdateEntityAsync(user);
                result.Message = "Nombre actualizado";
                result.IsSuccess = true;
                result.Data = user;
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
                result.Message = "Rol de usuario actualizado al usuario";
                result.Data = user;
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
                await _userRepository.UpdateEntityAsync(usuario);
                result.IsSuccess = true;
                result.Message = "Datos actualizados";
                result.Data = usuario;
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
                result.Message = "Clave actualizada";
                result.Data = usuario;
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
                throw new ArgumentNullException("El id del usuario  debe ser mayor que 0");
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
        private bool ValidateClave(string? clave)
        {
            if (string.IsNullOrEmpty(clave))
                return false;

            if (clave.Length < 12 || clave.Length > 50)
                return false;

            if (!clave.Any(char.IsUpper) || !clave.Any(char.IsDigit) || !clave.Any(char.IsLower))
                return false;

            string caracteresEspeciales = "@#!*?$/,{}=.;:";
            if (!clave.Any(c => caracteresEspeciales.Contains(c)))
                return false;

            if (clave.Contains(" "))
                return false;

            return true;
        }
    }
}
