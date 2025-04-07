using WebApi.Interfaces;
using WebApi.Interfaces.IUsersServices;
using WebApi.Models;
using WebApi.Models.UsersModels;

namespace WebApi.Repositories.UsersRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IApiClient _client;
        private readonly string _baseController = "User/";
        public UserRepository(IApiClient client)
        {
            _client = client;
        }
        public async Task<OperationResult> CreateAsync(UserModel entity)
        {
            var user = await _client.PostAsync<UserModel>($"{_baseController}user", entity);
            return user;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            return await _client.DeleteAsync($"{_baseController}user/{id}", id);
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            var usuarios = await _client.GetAsync<IEnumerable<UserModel>>($"{_baseController}users");
            return usuarios;
        }

        public async Task<UserModel> GetByIdAsync(int id)
        {
            return await _client.GetByIdAsync<UserModel>($"{_baseController}user/", id);
        }

        public async Task<UserModel> GetUserByDocumentAsync(string documento)
        {
            return await _client.GetAsync<UserModel>($"{_baseController}user/document?documento={documento}");
        }

        public async Task<UserModel> GetUserByEmailAsync(string correo)
        {
            var user = await _client.GetAsync<UserModel>($"{_baseController}user/email?correo={correo}");
                return user;
        }
        public async Task<List<UserModel>> GetUsersByName(string nombreCompleto)
        {
            return await _client.GetAsync<List<UserModel>>($"{_baseController}user/complete-name?nombreCompleto={nombreCompleto}");
        }

        public async Task<List<UserModel>> GetUsersByTypeDocumentAsync(string tipoDocumento)
        {
            return await _client.GetAsync<List<UserModel>>($"{_baseController}user/type-document?tipoDocumento={tipoDocumento}");
        }

        public async Task<OperationResult> UpdateAsync(int id, UserModel entity)
        {
            var user = await _client.PutAsync<UserModel>($"{_baseController}user/", id, entity);
            return user;
        }
    }
}
