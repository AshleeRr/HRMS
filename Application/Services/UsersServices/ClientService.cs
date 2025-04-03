using HRMS.Application.DTOs.UsersDTOs.ClientDTOs;
using HRMS.Application.DTOs.UsersDTOs.UserDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;
//using HRMS.Infraestructure.Notification;

namespace HRMS.Application.Services.UsersServices
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILoggingServices _loggingServices;
        private readonly IValidator<SaveClientDTO> _validator;
        private readonly IUserRepository _userRepository;
        //private readonly INotificationService _notificationService;
        public ClientService(IClientRepository clientRepository, IValidator<SaveClientDTO> validator, //INotificationService notificationService,
                                                                                                      ILoggingServices loggingServices, IUserRepository userRepository)
        {
            _clientRepository = clientRepository;
            _validator = validator;
            _loggingServices = loggingServices;
            _userRepository = userRepository;
            //_notificationService = notificationService;
        }

        //mappers
        private Client MapSaveToDto(SaveClientDTO dto) { 
            return new Client()
            {
                IdUsuario = dto.IdUsuario,
                NombreCompleto = dto.NombreCompleto,
                Correo = dto.Correo,
                Clave = dto.Clave,
                Documento = dto.Documento,
                TipoDocumento = dto.TipoDocumento,
                FechaCreacion = DateTime.Now,
                UserID = dto.UserID,
            };
        }

        private ClientViewDTO MapClientToViewDto(Client client)
        {
            return new ClientViewDTO()
            {
                IdCliente = client.IdCliente,
                IdUsuario = client.IdUsuario,
                NombreCompleto = client.NombreCompleto,
                Correo = client.Correo,
                Clave = client.Clave,
                Documento = client.Documento,
                TipoDocumento = client.TipoDocumento,
                ChangeTime = client.FechaCreacion,
                UserID = client.UserID 
            };
        }
        //methods
        public async Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();
            try
            {
                var clients = await _clientRepository.GetAllAsync();
                if (!clients.Any())
                {
                    result.IsSuccess = false;
                    result.Data = new List<ClientViewDTO>();
                    result.Message = "No hay clientes registrados";
                }
                else
                {
                    result.IsSuccess = true;
                    result.Data = clients.Select(MapClientToViewDto).ToList();
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
                result.Data = MapClientToViewDto(client);
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
                ValidateId(dto.IdUsuario);
                await ValidateUserIDAsync(dto.UserID);
                var client = await _clientRepository.GetEntityByIdAsync(dto.IdUsuario);
                ValidateClient(client);
                client.Estado = false;
                result = await _clientRepository.UpdateEntityAsync(client);
                if(!result.IsSuccess)
                {
                    result.Message = "Error eliminando el cliente";
                }
                else
                {
                    result.Data = dto;
                    result.Message = "Cliente eliminado correctamente";
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
        public async Task<OperationResult> GetClientByUserIdAsync(int idUsuario)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(idUsuario);
                var client = await _clientRepository.GetClientByUserIdAsync(idUsuario);
                if (client == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un cliente con este id";
                }
                else
                {
                    result.Data = client;
                    result.Message = "Cliente encontrado correctamente";
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
        public async Task<OperationResult> Save(SaveClientDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validDTO = _validator.Validate(dto);
                if (!validDTO.IsSuccess)
                {
                    result.Message = "Error validando los datos para guardar";
                }
                else
                {
                    var existingCorreo = await _clientRepository.GetClientByEmailAsync(dto.Correo);
                    var existingDocument = await _clientRepository.GetClientByDocumentAsync(dto.Documento);
                    ValidateExistingData(existingCorreo, "correo");
                    ValidateExistingData(existingDocument, "documento");
                    await ValidateUserIDAsync(dto.UserID);
                    var client = MapSaveToDto(dto);

                    result = await _clientRepository.SaveEntityAsync(client);
                    if (result.IsSuccess)
                    {
                        result.Message = "Cliente guardado correctamente";
                        result.Data = MapClientToViewDto(client);
                        //_notificationService.SendNotification(dto.IdUsuario, $"Bienvenido, {dto.NombreCompleto}!\nGracias por registrarte en la aplicación de nuestro hotel.\nCorreo registrado: {dto.Correo}\nSi necesitas asistencia, no dudes en contactarnos.");
                    }
                    else
                    {
                        result.Message = "Error guardando el cliente";
                    }
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
                var client = await _clientRepository.GetClientByUserIdAsync(dto.IdUsuario);
                ValidateClient(client);
                var existingCorreo = await _clientRepository.GetClientByEmailAsync(dto.Correo);
                var existingDocument = await _clientRepository.GetClientByDocumentAsync(dto.Documento);
                ValidateExistingData(existingCorreo, "correo");
                ValidateExistingData(existingDocument, "documento");
                await ValidateUserIDAsync(dto.UserID);
                client.IdUsuario = dto.IdUsuario;
                client.NombreCompleto = dto.NombreCompleto;
                client.TipoDocumento = dto.TipoDocumento;
                client.Documento = dto.Documento;
                client.Clave = dto.Clave;
                client.Correo = dto.Correo;
                client.FechaCreacion = dto.ChangeTime;
                client.UserID = dto.UserID;
                result = await _clientRepository.UpdateEntityAsync(client);
                if (result.IsSuccess)
                {
                    result.Message = "Cliente actualizado correctamente";
                    result.Data = MapClientToViewDto(client);
                }
                else
                {
                    result.Message = "Error actualizando el cliente";
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
                    result.Message = "Error actualizando la clave del cliente";
                }
                else
                {
                    result.Data = MapClientToViewDto(client);
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
                ValidateExistingData(existingCorreo, "correo");
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
                    result.Data = MapClientToViewDto(client);
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
                    result.Message = "Error actualizando el nombre del cliente";
                }
                else
                {
                    result.Data = MapClientToViewDto(client);
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
                ValidateExistingData(existingDocument, "documento");
                var cliente = await _clientRepository.GetClientByUserIdAsync(id);
                ValidateClient(cliente);
                cliente.Documento = documento;
                cliente.TipoDocumento = tipoDocumento;
                result = await _clientRepository.UpdateEntityAsync(cliente);
                if (!result.IsSuccess)
                {
                    result.Message = "Error actualizando los datos";
                }
                else
                {
                    result.Data = MapClientToViewDto(cliente);
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
        private async Task ValidateUserIDAsync(int userID)
        {
            ValidateId(userID);

            bool userExists = await _userRepository.UserIDExistsAsync(userID);
            if (!userExists)
            {
                throw new ArgumentException("El UserID ingresado no está registrado. No se puede proceder con el proceso");
            }
        }
        private void ValidateExistingData(OperationResult client, string data)
        {
            if (client.IsSuccess)
            {
                throw new ArgumentException($"Este {data} ya esta registrado");
            }
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
