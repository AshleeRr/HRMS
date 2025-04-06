using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Repositories.RoomRepositories;

public class HabitacionRepository : IHabitacionRepository
{
    private readonly IApiClient _apiClient;

    public HabitacionRepository(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private const string BaseEndpoint = "Habitacion";
    public async Task<IEnumerable<HabitacionModel>> GetAllAsync()
    {
        var result = await _apiClient.GetAsync<IEnumerable<HabitacionModel>>($"{BaseEndpoint}/GetAllHabitaciones");
        return result ?? (Enumerable.Empty<HabitacionModel>());
    }

    public async Task<HabitacionModel> GetByIdAsync(int id)
    {
        var result = await _apiClient.GetByIdAsync<HabitacionModel>($"{BaseEndpoint}/GetByHabitacionById", id);
        return result ?? (new HabitacionModel
        {
            idHabitacion = -1,
            detalle = "No se pudo obtener la habitación"
        });
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

        return _apiClient.PutAsync($"{BaseEndpoint}/UpdateHabitacion", id, entity);
    }

    public Task<OperationResult> DeleteAsync(int id)
    {
        return _apiClient.DeleteAsync($"{BaseEndpoint}/DeleteHabitacion", id);
    }

    public async Task<HabitacionModel> GetByNumeroAsync(string numero)
    {
        var result =  await _apiClient.GetAsync<HabitacionModel>($"{BaseEndpoint}/GetHabitacionBy/{numero}");
        return result ?? (new HabitacionModel
        {
            idHabitacion = -1,
            detalle = "No se pudo encontrar la habitación por número"
        });
    }

    public Task<IEnumerable<HabitacionModel>> GetByCategoriaAsync(string categoria)
    {
        var result = _apiClient.GetAsync<IEnumerable<HabitacionModel>>($"{BaseEndpoint}/GetHabitacionByCategoria/{categoria}");
        return result ?? (Task.FromResult(Enumerable.Empty<HabitacionModel>()));
    }

    public async Task<IEnumerable<HabitacionModel>> GetByPisoAsync(int pisoId)
    {
        var result = await _apiClient.GetAsync<IEnumerable<HabitacionModel>>($"{BaseEndpoint}/GetHabitacionByPiso/{pisoId}");
        return result ?? (Enumerable.Empty<HabitacionModel>());
    }

    public async Task<IEnumerable<HabitacionInfoModel>> GetInfoHabitaciones()
    {
        var result = await _apiClient.GetAsync<IEnumerable<HabitacionInfoModel>>($"{BaseEndpoint}/GetInfoHabitaciones");
        return result ?? (Enumerable.Empty<HabitacionInfoModel>());
    }
}