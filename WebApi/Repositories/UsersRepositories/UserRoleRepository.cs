using Newtonsoft.Json;
using WebApi.Interfaces;
using WebApi.Interfaces.IUsersServices;
using WebApi.Models;
using WebApi.Models.UsersModels;

namespace WebApi.Repositories.UsersRepositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly IApiClient _client;
        private readonly string _baseController = "UserRole/";
        public UserRoleRepository(IApiClient client)
        {
            _client = client;
        }
        public async Task<OperationResult> CreateAsync(UserRoleModel entity)
        {
            var role = await _client.PostAsync<UserRoleModel>($"{_baseController}role", entity);
            return role;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            return await _client.DeleteAsync($"{_baseController}role/{id}", id);
        }

        public async Task<IEnumerable<UserRoleModel>> GetAllAsync()
        {
            return await _client.GetAsync<IEnumerable<UserRoleModel>>($"{_baseController}roles");
        }

        public async Task<UserRoleModel> GetByIdAsync(int id)
        {
            var role = await _client.GetByIdAsync<UserRoleModel>($"{_baseController}role/", id);
            return role;
        }

        public async Task<UserRoleModel> GetRoleByDescription(string descripcion)
        {
            return await _client.GetAsync<UserRoleModel>($"{_baseController}role/description?descripcion={descripcion}");
        }

        public async Task<UserRoleModel> GetRoleByName(string nombre)
        {
            return await _client.GetAsync<UserRoleModel>($"{_baseController}role/name?nombre={nombre}");
        }

        public async Task<List<UserModel>> GetUsersByRole(int id)
        {

            var roles = await _client.GetAsync<List<UserModel>>($"{_baseController}role/{id}/users");
            return roles;

        }

        public async Task<OperationResult> UpdateAsync(int id, UserRoleModel entity)
        {
            var rol = await _client.PutAsync<UserRoleModel>($"{_baseController}role/", id, entity);
            return rol;
        }
    }
}
