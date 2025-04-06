using System.Collections.Generic;
using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models;
using WebApi.Models.RoomModels;


namespace WebApi.Repositories
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
            return await _apiClient.GetAsync<IEnumerable<CategoriaModel>>($"{BaseEndpoint}/GetAllCategorias");
        }

        public async Task<CategoriaModel> GetByIdAsync(int id)
        {
            return await _apiClient.GetByIdAsync<CategoriaModel>($"{BaseEndpoint}/GetCategoriaById", id);
        }

        public async Task<CategoriaModel> GetByDescripcionAsync(string descripcion)
        {
            return await _apiClient.GetAsync<CategoriaModel>($"{BaseEndpoint}/GetCategoriaByDescripcion/{descripcion}");
        }

        public async Task<IEnumerable<CategoriaModel>> GetByCapacidadAsync(int capacidad)
        {
            return await _apiClient.GetAsync<IEnumerable<CategoriaModel>>($"{BaseEndpoint}/GetHabitacionByCapacidad/{capacidad}");
        }

        public async Task<OperationResult> CreateAsync(CategoriaModel categoria)
        {
            return await _apiClient.PostAsync($"{BaseEndpoint}/CreateCategoria", categoria);
        }

        public async Task<OperationResult> UpdateAsync(int id, CategoriaModel categoria)
        {
            return await _apiClient.PutAsync($"{BaseEndpoint}/UpdateCategoriaById", id, categoria);
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                return await _apiClient.DeleteAsync($"{BaseEndpoint}/DeleteCategoriaById", id);
            }
            catch (ApiException ex)
            {
                return new OperationResult 
                { 
                    IsSuccess = false, 
                    Message = ex.Message 
                };
            }
        }
    }
}