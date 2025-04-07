using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Repositories.RoomRepositories;

public class HabitacionRepository : IHabitacionRepository
{
    private readonly IApiClient _apiClient;
    private const string BaseEndpoint = "Habitacion";

    public HabitacionRepository(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    
    public async Task<IEnumerable<HabitacionModel>> GetAllAsync()
    {
        var result = await _apiClient.GetAsync<IEnumerable<HabitacionModel>>($"{BaseEndpoint}/GetAllHabitaciones");
        return result ?? Enumerable.Empty<HabitacionModel>();
    }

    public async Task<HabitacionModel> GetByIdAsync(int id)
    {
        var endpoint = $"{BaseEndpoint}/GetByHabitacionById";
        var result = await _apiClient.GetAsync<HabitacionModel>($"{endpoint}{id}");
        
        return result ?? new HabitacionModel
        {
            idHabitacion = -1,
            detalle = "No se pudo obtener la habitación"
        };
    }

    public Task<OperationResult> CreateAsync(HabitacionModel entity)
    {
        return _apiClient.PostAsync($"{BaseEndpoint}/CreateHabitacion", entity);
    }

    public Task<OperationResult> UpdateAsync(int id, HabitacionModel entity)
    {
        if (entity.idHabitacion != id)
        {
            entity.idHabitacion = id;
        }
        
        // Formato exacto del endpoint: (UpdateHabitacionBy)1 (con paréntesis)
        var endpoint = $"{BaseEndpoint}/(UpdateHabitacionBy)";
        return _apiClient.PutAsync(endpoint, id, entity);
    }

    public Task<OperationResult> DeleteAsync(int id)
    {
        // Formato exacto del endpoint: DeleteHabitacionBy0 (sin separador)
        var endpoint = $"{BaseEndpoint}/DeleteHabitacionBy";
        return _apiClient.DeleteAsync(endpoint, id);
    }

    public async Task<HabitacionModel> GetByNumeroAsync(string numero)
    {
        // Formato exacto del endpoint: GetHabitacionBy/101 (con separador)
        var result = await _apiClient.GetAsync<HabitacionModel>($"{BaseEndpoint}/GetHabitacionBy/{numero}");
        
        return result ?? new HabitacionModel
        {
            idHabitacion = -1,
            detalle = "No se pudo encontrar la habitación por número"
        };
    }

    public async Task<IEnumerable<HabitacionModel>> GetByCategoriaAsync(string categoria)
    {
        var result = await _apiClient.GetAsync<IEnumerable<HabitacionModel>>($"{BaseEndpoint}/GetHabitacionByCategoria/{categoria}");
        return result ?? Enumerable.Empty<HabitacionModel>();
    }

    public async Task<IEnumerable<HabitacionModel>> GetByPisoAsync(int pisoId)
    {
        var endpoint = $"{BaseEndpoint}/GetHabitacionByPiso/{pisoId}";
        Console.WriteLine($"Intentando acceder a: {endpoint}");
    
        var result = await _apiClient.GetAsync<IEnumerable<HabitacionModel>>($"GetHabitacionByPiso/{pisoId}" );
    
        return result ?? Enumerable.Empty<HabitacionModel>();
    }

    public async Task<IEnumerable<HabitacionInfoModel>> GetInfoHabitaciones()
    {
        var result = await _apiClient.GetAsync<IEnumerable<HabitacionInfoModel>>($"{BaseEndpoint}/GetInfoHabitaciones");
        return result ?? Enumerable.Empty<HabitacionInfoModel>();
    }
}