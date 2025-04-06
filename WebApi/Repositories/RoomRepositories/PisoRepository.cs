using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Repositories.RoomRepositories
{
    public class PisoRepository : IPisoRepository
    {
        private readonly IApiClient _apiClient;
        private const string BaseEndpoint = "Piso";

        public PisoRepository(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        
        public async Task<IEnumerable<PisoModel>> GetAllAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<PisoModel>>($"{BaseEndpoint}/GetAllPisos");
            return result ?? Enumerable.Empty<PisoModel>();
        }

        public async Task<PisoModel> GetByIdAsync(int id)
        {
            var result = await _apiClient.GetByIdAsync<PisoModel>($"{BaseEndpoint}/GetPisoById", id);
            return result ?? new PisoModel
            {
                IdPiso = -1,
                Descripcion = "No se pudo obtener el piso"
            };
        }

        public Task<OperationResult> CreateAsync(PisoModel entity)
        {
            return _apiClient.PostAsync($"{BaseEndpoint}/CreatePiso", entity);
        }

        public Task<OperationResult> UpdateAsync(int id, PisoModel entity)
        {
            if (entity.IdPiso != id)
            {
                entity.IdPiso = id;
            }
            
            return _apiClient.PatchAsync($"{BaseEndpoint}/UpdatePiso", id, entity);
        }

        public Task<OperationResult> DeleteAsync(int id)
        {
            return _apiClient.DeleteAsync($"{BaseEndpoint}/DeletePiso", id);
        }

        public async Task<PisoModel> GetByDescripcionAsync(string descripcion)
        {
            var result = await _apiClient.GetAsync<PisoModel>($"{BaseEndpoint}/GetPisoByDescripcion/{descripcion}");
            return result ?? new PisoModel
            {
                IdPiso = -1,
                Descripcion = "No se pudo encontrar el piso por descripción"
            };
        }
    }
}