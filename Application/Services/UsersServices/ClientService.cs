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

        public async Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        public async Task<OperationResult> GetById(int id)
        {
            OperationResult result = new OperationResult();
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        public async Task<OperationResult> Remove(RemoveClientDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        public async Task<OperationResult> Save(SaveClientDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        public async Task<OperationResult> UpdateTipoDocumentoAndDocumentoAsync(int idCliente, int idTipoDocumento, string documento)
        {
            OperationResult result = new OperationResult();
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        public async Task<OperationResult> Update(UpdateClientDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
    }
}
