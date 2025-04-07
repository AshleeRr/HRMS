using WebApi.Interfaces;
using WebApi.Interfaces.IUsersServices;
using WebApi.Models.UsersModels;

namespace WebApi.Repositories.UsersRepositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IApiClient _client;
        private readonly string _baseController = "Client/";
        public ClientRepository(IApiClient client)
        {
            _client = client;
        }
        public async Task<IEnumerable<ClientModel>> GetAllAsync()
        {
            var clients = await _client.GetAsync<IEnumerable<ClientModel>>($"{_baseController}clients");
            return clients;
        }

        public async Task<ClientModel> GetByIdAsync(int id)
        {
            return await _client.GetAsync<ClientModel>($"{_baseController}client/{id}");
        }

        public async Task<ClientModel> GetClientByDocument(string document)
        {
            return await _client.GetAsync<ClientModel>($"{_baseController}client/document?document={document}");
        }

        public async Task<ClientModel> GetClientByEmail(string email)
        {
            return await _client.GetAsync<ClientModel>($"{_baseController}client/email?email={email}");
        }

        public async Task<List<ClientModel>> GetClientsByDocumentType(string tipoDocumento)
        {
            return await _client.GetAsync<List<ClientModel>>($"{_baseController}user/type-document?tipoDocumento={tipoDocumento}");
        }
    }
}
