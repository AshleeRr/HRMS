using HRMS.Application.DTOs.ClientDTOs;
using HRMS.Application.DTOs.UserDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;

namespace HRMS.Application.Services.UsersServices
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILoggingServices _loggingServices;
        private readonly IValidator<SaveClientDTO> _validator;
        public ClientService(IClientRepository clientRepository, IValidator<SaveClientDTO> validator, ILoggingServices loggingServices)
        {
            _clientRepository = clientRepository;
            _validator = validator;
            _loggingServices = loggingServices;
        }

        public async Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();
            try
            {
                var clients = await _clientRepository.GetAllAsync();
                if (!clients.Any())
                {
                    result.IsSuccess = false;
                    result.Message = "No hay clientes registrados";
                }
                else
                {
                    result.IsSuccess = true;
                    result.Data = clients;
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(id);
                var client = await _clientRepository.GetEntityByIdAsync(id);
                ValidateClient(client);
                result.IsSuccess = true;
                result.Data = client;
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            catch (Exception ex) 
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> Remove(RemoveUserClientDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(dto.Id);
                var client = await _clientRepository.GetEntityByIdAsync(dto.Id);
                ValidateClient(client);
                dto.Deleted = true;
                client.Estado = false;
                result = await _clientRepository.UpdateEntityAsync(client);
                result.IsSuccess = true;
                result.Message = "Cliente eliminado correctamente";
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<Client> GetClientByUserIdAsync(int idUsuario)
        {
            return await _clientRepository.GetClientByUserIdAsync(idUsuario);
        }
        public async Task<OperationResult> Save(SaveClientDTO dto)
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
                var existingCorreo = await _clientRepository.GetClientByEmailAsync(dto.Correo);
                if (existingCorreo != null)
                {
                    result.Message = "Este correo ya esta registrado";
                    result.IsSuccess = false;
                    return result;
                }
                var existingDocument = await _clientRepository.GetClientByDocumentAsync(dto.Documento);
                if (existingDocument != null)
                {
                    result.Message = "Este documento ya esta registrado";
                    result.IsSuccess = false;
                    return result;
                }
                var client = new Client()
                {
                    IdUsuario = dto.IdUsuario,
                    NombreCompleto = dto.NombreCompleto,
                    Correo = dto.Correo,
                    Clave = dto.Clave,
                    Documento = dto.Documento,
                    TipoDocumento = dto.TipoDocumento,
                };

                result = await _clientRepository.SaveEntityAsync(client);
                if (result.IsSuccess)
                {
                    result.Message = "Cliente guardado correctamente";
                    result.Data = dto;
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> Update(UpdateUserClientDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(dto.IdUsuario);
                var client = await _clientRepository.GetEntityByIdAsync(dto.IdUsuario);
                ValidateClient(client);
                var existingCorreo = await _clientRepository.GetClientByEmailAsync(dto.Correo);
                if (existingCorreo != null && existingCorreo.IdUsuario != dto.IdUsuario)
                {
                    result.Message = "Este correo ya esta registrado";
                    result.IsSuccess = true;
                }
                var existingDocument = await _clientRepository.GetClientByDocumentAsync(dto.Documento);
                if (existingDocument != null && existingDocument.IdUsuario != dto.IdUsuario)
                {
                    result.Message = "Este documento ya esta registrado";
                    result.IsSuccess = true;
                }
                client.IdUsuario = dto.IdUsuario;
                client.NombreCompleto = dto.NombreCompleto;
                client.TipoDocumento = dto.TipoDocumento;
                client.Documento = dto.Documento;
                client.Clave = dto.Clave;
                client.Correo = dto.Correo;
                dto.ChangeTime = DateTime.Now;
                await _clientRepository.UpdateEntityAsync(client);
                result.Message = "Cliente actualizado correctamente";
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
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<OperationResult> UpdatePasswordAsync(int id, string nuevaClave)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(id);
                ValidateClave(nuevaClave);
                var client = await _clientRepository.GetClientByUserIdAsync(id);
                ValidateClient(client);
                client.Clave = nuevaClave;
                result = await _clientRepository.UpdateEntityAsync(client);
                if(!result.IsSuccess)
                {
                    result.Message = "Error actualizando la clave cliente";
                }
                else
                {
                    result.Message = "Se actualizó la clave del cliente";
                }
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> UpdateCorreoAsync(int id, string nuevoCorreo)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(id);
                ValidateNulleable(nuevoCorreo, "nuevo correo");
                var existingCorreo = await _clientRepository.GetClientByEmailAsync(nuevoCorreo);
                if (existingCorreo != null && existingCorreo.IdUsuario != id)
                {
                    result.Message = "Este correo ya esta registrado";
                    result.IsSuccess = true;
                }
                var client = await _clientRepository.GetClientByUserIdAsync(id);
                ValidateClient(client);
                client.Correo = nuevoCorreo;
                result = await _clientRepository.UpdateEntityAsync(client);
                if (!result.IsSuccess)
                {
                    result.Message = "Error actualizando el correo del cliente";
                }
                else
                {
                    result.Message = "Se actualizó el correo del cliente";
                }
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> UpdateNombreCompletoAsync(int id, string nuevoNombreCompleto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(id);
                ValidateNulleable(nuevoNombreCompleto, "nuevo nombre completo");

                var client = await _clientRepository.GetClientByUserIdAsync(id);
                ValidateClient(client);
                client.NombreCompleto = nuevoNombreCompleto;
                result = await _clientRepository.UpdateEntityAsync(client);
                if (!result.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "Error actualizando el nombre del cliente";
                }
                else
                {
                    result.Message = "Se actualizó el nombre del cliente";
                }
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> UpdateTipoDocumentoAndDocumentoAsync(int id, string tipoDocumento, string documento)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(id);
                ValidateNulleable(documento, "documento");
                ValidateNulleable(tipoDocumento, "tipo documento");
                var existingDocument = await _clientRepository.GetClientByDocumentAsync(documento);
                if (existingDocument != null && existingDocument.IdUsuario != id)
                {
                    result.Message = "Este documento ya esta registrado";
                    result.IsSuccess = true;
                }
                var cliente = await _clientRepository.GetClientByUserIdAsync(id);
                ValidateClient(cliente);
                cliente.Documento = documento;
                cliente.TipoDocumento = tipoDocumento;
                await _clientRepository.UpdateEntityAsync(cliente);
                if (!result.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "Error actualizando los datos";
                }
                else
                {
                    result.Message = "Datos actualizados correctamente";
                }
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }
        private int ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El id debe ser mayor que 0");
            }
            return id;
        }
        private Client ValidateClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("No existe un cliente con este id");
            }
            return client;
        }
        private void ValidateNulleable(string x, string message)
        {
            if (string.IsNullOrEmpty(x))
            {
                throw new ArgumentException($"El campo: {message} no puede estar vacio.");
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
