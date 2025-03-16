using HRMS.Application.DTOs.ClientDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
 
namespace HRMS.Application.Services.UsersServices
{
    public class ClientService : IClientService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientService> _logger;
        private readonly IClientRepository _clientRepository;
        public ClientService(IClientRepository clientRepository,
                                ILogger<ClientService> logger, IConfiguration configuration)
        {
            _clientRepository = clientRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public Task<OperationResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Remove(RemoveClientDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Save(SaveClientDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> UpdataTipoDocumentoAndDocumentoAsync(int idCliente, int idTipoDocumento, string documento)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Update(UpdateClientDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
