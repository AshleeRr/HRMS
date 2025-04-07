using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Repositories.RoomRepositories;

public class TarifaRepository : ITarifaRepository
{
    private readonly IApiClient _apiClient;
    private const string BaseEndpoint = "Tarifa";
    
    public TarifaRepository(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IEnumerable<TarifaModel>> GetAllAsync()
    {
        var result = await _apiClient.GetAsync<IEnumerable<TarifaModel>>($"{BaseEndpoint}/GetAllTarifas");
        return result ?? Enumerable.Empty<TarifaModel>();
    }

    public async Task<TarifaModel> GetByIdAsync(int id)
    {
        var result = await _apiClient.GetByIdAsync<TarifaModel>("Tarifa/GetTarifaById", id);
        return result ?? new TarifaModel
        {
            IdTarifa = -1,
            Descripcion = "No se pudo obtener la tarifa"
        };
    }

    public async Task<OperationResult> CreateAsync(TarifaModel entity)
    {
        return await _apiClient.PostAsync($"{BaseEndpoint}/CreateTarifa", entity);
    }

    public async Task<OperationResult> UpdateAsync(int id, TarifaModel entity)
    {
        if (entity.IdTarifa != id)
        {
            entity.IdTarifa = id;
        }
        
        return await _apiClient.PutAsync($"{BaseEndpoint}/UpdateTarifaBy", id, entity);
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        return await _apiClient.DeleteAsync($"{BaseEndpoint}/DeleteTarifaBy", id);
    }

    public async Task<IEnumerable<TarifaModel>> GetTarifaByPrecio(decimal precio)
    {
        var result = await _apiClient.GetAsync<TarifaModel>($"{BaseEndpoint}/GetTarifaByPrecio/{precio}");
        return result != null ? new List<TarifaModel> { result } : Enumerable.Empty<TarifaModel>();
    }

    public async Task<IEnumerable<TarifaModel>> GetTarifaByFecha(DateTime fecha)
    {
        var result = await _apiClient.GetAsync<IEnumerable<TarifaModel>>($"{BaseEndpoint}/GetTarifaByFecha/{fecha}");
        return result ?? Enumerable.Empty<TarifaModel>();
    }
}