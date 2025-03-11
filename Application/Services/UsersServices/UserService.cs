using HRMS.Application.DTOs.UserDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;

namespace HRMS.Application.Services.UsersServices
{
    public class UserService : IUserService
    {
        private readonly ILoggingServices _loggerServices;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        public UserService(IUserRepository userRepository,
                                ILoggingServices loggerServices)
        {
            _userRepository = userRepository;
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
        public async Task<OperationResult> Remove(RemoveUserDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateUserDto(dto);
                var user = await _userRepository.GetEntityByIdAsync(dto.IdUsuario);
                ValidateUser(user);
                dto.Deleted = true;
                user.Estado = false;
                if(dto.IdUsuario == 1)
                {
                    var cliente = await _clientRepository.GetEntityByIdAsync(dto.IdUsuario);
                    ValidateClient(cliente);
                    cliente.Estado = false;
                    await _clientRepository.UpdateEntityAsync(cliente);
                }
                await _userRepository.UpdateEntityAsync(user);
                result.Message = "Usuario eliminado";
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
                ValidateUserDto(dto);
                ValidateClave(dto.Clave);
                ValidateId(dto.IdUserRole);
                var existingEmail = await _userRepository.GetUserByEmailAsync(dto.Correo);
                var existingDocument = await _userRepository.GetUserByDocumentAsync(dto.Documento);
                ValidateExistence(existingEmail, "Este correo ya está registrado");
                ValidateExistence(existingDocument, "Este documento ya está registrado");

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
                await _userRepository.SaveEntityAsync(usuario);
                
                if (dto.IdUserRole == 1)
                {
                    var cliente = new Client
                    {
                        IdUsuario = usuario.IdUsuario,
                        NombreCompleto = dto.NombreCompleto,
                        Correo = dto.Correo,
                        Documento = dto.Documento,
                        TipoDocumento = dto.TipoDocumento,
                        Clave = dto.Clave,
                        FechaCreacion = DateTime.Now
                    };
                    await _clientRepository.SaveEntityAsync(cliente);
                }
                result.Message = "Usuario guardado";
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<OperationResult> Update(UpdateUserDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateUserDto(dto);
                ValidateId(dto.IdUsuario);
                ValidateClave(dto.Clave);
                ValidateId(dto.IdUserRole);
                var user = await _userRepository.GetEntityByIdAsync(dto.IdUsuario);
                var existingEmail = await _userRepository.GetUserByEmailAsync(dto.Correo);
                var existingDocument = await _userRepository.GetUserByDocumentAsync(dto.Documento);
                ValidateUser(user);
                ValidateExistence(existingEmail, "Este correo ya está registrado");
                ValidateExistence(existingDocument, "Este documento ya está registrado");
                if (user.IdRolUsuario == 1)
                {
                    var cliente = await _clientRepository.GetEntityByIdAsync(dto.IdUsuario);
                    ValidateClient(cliente);
                    cliente.Correo = dto.Correo;
                    cliente.NombreCompleto = dto.NombreCompleto;
                    cliente.TipoDocumento = dto.TipoDocumento;
                    cliente.Documento = dto.Documento;
                    cliente.Clave = dto.Clave;
                    dto.ChangeTime = DateTime.Now;
                    await _clientRepository.UpdateEntityAsync(cliente);
                }
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
                ValidateNulleable(nuevoCorreo, "El nuevo correo no puede ser nulo");
                var user = await _userRepository.GetEntityByIdAsync(idUsuario);
                ValidateUser(user);
                if (user.IdRolUsuario == 1)
                {
                    var cliente = await _clientRepository.GetEntityByIdAsync(idUsuario);
                    ValidateClient(cliente);
                    if (cliente.Correo == nuevoCorreo)
                    {
                        result.IsSuccess = false;
                        result.Message = "Este correo ya existe";
                    }
                    cliente.Correo = nuevoCorreo;
                    await _clientRepository.UpdateEntityAsync(cliente);
                    result.IsSuccess = true;
                }
                if (user.Correo == nuevoCorreo)
                {
                    result.IsSuccess = false;
                    result.Message = "Este correo ya existe";
                }
                user.Correo = nuevoCorreo;
                await _userRepository.UpdateEntityAsync(user);
                result.Message = "Correo actualizado";
                result.IsSuccess = true;
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
                ValidateNulleable(nuevoNombreCompleto, "El nuevo nombre no puede ser nulo");
                var user = await _userRepository.GetEntityByIdAsync(idUsuario);
                if (user.IdUsuario == 1)
                {
                    var cliente = await _clientRepository.GetEntityByIdAsync(idUsuario);
                    ValidateClient(cliente);
                    cliente.NombreCompleto = nuevoNombreCompleto;
                    await _clientRepository.UpdateEntityAsync(cliente);
                }
                user.NombreCompleto = nuevoNombreCompleto;
                await _userRepository.UpdateEntityAsync(user);
                result.Message = "Nombre actualizado";
                result.IsSuccess = true;
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
                ValidateNulleable(tipoDocumento, "El tipo de documento no puede estar vacio");
                ValidateNulleable(documento, "El documento no puede estar vacio");
                var usuario = await _userRepository.GetEntityByIdAsync(idUsuario);
                ValidateUser(usuario);
                if(usuario.IdRolUsuario == 1)
                {
                    var cliente = await _clientRepository.GetEntityByIdAsync(idUsuario);
                    ValidateClient(cliente);
                    var existingClientDocument = await _clientRepository.GetClientByDocumentAsync(documento);
                    ValidateExistence(documento, "Este documento ya esta siendo usado");
                    cliente.TipoDocumento = documento;
                    cliente.Documento = documento;
                    await _clientRepository.UpdateEntityAsync(cliente);
                    result.IsSuccess = true;
                }
                var existingDocument = await _userRepository.GetUserByDocumentAsync(documento);
                ValidateExistence(documento, "Este documento ya esta siendo usado");
                usuario.TipoDocumento = tipoDocumento;
                usuario.Documento = documento;
                await _userRepository.UpdateEntityAsync(usuario);
                result.IsSuccess = true;
                result.Message = "Datos actualizados";
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
                if(usuario.IdUsuario == 1)
                {
                    var cliente = await _clientRepository.GetEntityByIdAsync(idUsuario);
                    ValidateClient(cliente);
                    cliente.Clave = nuevaClave;
                    await _clientRepository.UpdateEntityAsync(cliente); 
                    result.IsSuccess = true;
                }
                usuario.Clave = nuevaClave;
                await _userRepository.UpdateEntityAsync(usuario);
                result.IsSuccess = true;
                result.Message = "Clave actualizada";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        // metodos de validacion
        private void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("El id del usuario  debe ser mayor que 0");
            }
        }
        private void ValidateExistence(object existingObject, string comment)
        {
            if (existingObject != null)
            {
                throw new ArgumentException(comment);
            }
        }
        private void ValidateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("El usuario no existe");
            }
        }
        private void ValidateUserDto(object dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("Dto nulo");
            }
        }
        private void ValidateClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("El cliente no existe");
            }
        }
        private void ValidateNulleable(string x, string message)
        {
            if (string.IsNullOrEmpty(x))
            {
                throw new ArgumentNullException(message);
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

            return true;
        }
    }
}
