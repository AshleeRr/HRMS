using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Repositories.RoomRepositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly IApiClient _apiClient;
        private const string BaseEndpoint = "Categoria";

        public CategoriaRepository(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<CategoriaModel>> GetAllAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<CategoriaModel>>($"{BaseEndpoint}/GetAllCategorias");
            return result ?? Enumerable.Empty<CategoriaModel>();
        }

        public async Task<CategoriaModel> GetByIdAsync(int id)
        {
            var result = await _apiClient.GetByIdAsync<CategoriaModel>($"{BaseEndpoint}/GetCategoriaById", id);
            return result ?? new CategoriaModel(); 
        }

        public async Task<CategoriaModel> GetByDescripcionAsync(string descripcion)
        {
            var result = await _apiClient.GetAsync<CategoriaModel>($"{BaseEndpoint}/GetCategoriaByDescripcion/{descripcion}");
            return result ?? new CategoriaModel();
        }

        public async Task<IEnumerable<CategoriaModel>> GetByCapacidadAsync(int capacidad)
        {
            var result = await _apiClient.GetAsync<IEnumerable<CategoriaModel>>($"{BaseEndpoint}/GetHabitacionByCapacidad/{capacidad}");
            return result ?? Enumerable.Empty<CategoriaModel>();
        }

        public Task<OperationResult> CreateAsync(CategoriaModel categoria)
        {
            return _apiClient.PostAsync($"{BaseEndpoint}/CreateCategoria", categoria);
        }

        public Task<OperationResult> UpdateAsync(int id, CategoriaModel categoria)
        {
            return _apiClient.PutAsync($"{BaseEndpoint}/UpdateCategoriaById", id, categoria);
        }

        public Task<OperationResult> DeleteAsync(int id)
        {
            return _apiClient.DeleteAsync($"{BaseEndpoint}/DeleteCategoriaById", id);
        }
    }
}