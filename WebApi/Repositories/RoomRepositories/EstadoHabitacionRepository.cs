using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Repositories.RoomRepositories;

public class EstadoHabitacionRepository : IEstadoHabitacionRepository
{
    private readonly IApiClient _apiClient;

    public EstadoHabitacionRepository(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private const string BaseEndpoint = "EstadoHabitacion";
    
    public async Task<IEnumerable<EstadoHabitacionModel>> GetAllAsync()
    {
        var result = await _apiClient.GetAsync<IEnumerable<EstadoHabitacionModel>>($"{BaseEndpoint}/GetEstadoHabitaciones");
        return result ?? Enumerable.Empty<EstadoHabitacionModel>();
    }
    
    public async Task<EstadoHabitacionModel> GetByIdAsync(int id)
    {
        var endpoint = $"{BaseEndpoint}/GetEstadoBy(id){id}";
        
        var result = await _apiClient.GetAsync<EstadoHabitacionModel>(endpoint);
        
        if (result == null || result.IdEstadoHabitacion <= 0)
        {
            var allEstados = await GetAllAsync();
            result = allEstados.FirstOrDefault(e => e.IdEstadoHabitacion == id);
            
            if (result != null)
            {
                return result;
            }
            
            return new EstadoHabitacionModel
            {
                IdEstadoHabitacion = -1,
                Descripcion = "Estado no disponible"
            };
        }
        
        return result;
    }

    public async Task<OperationResult> CreateAsync(EstadoHabitacionModel entity)
    {
        return await _apiClient.PostAsync($"{BaseEndpoint}/CreateEstadoHabitacion", entity);
    }

    public async Task<OperationResult> UpdateAsync(int id, EstadoHabitacionModel entity)
    {
        if (entity.IdEstadoHabitacion != id)
        {
            entity.IdEstadoHabitacion = id;
        }
        
        var endpoint = $"{BaseEndpoint}/UpdateEstadoHabitacionById";
        
        return await _apiClient.PatchAsync(endpoint, id, entity);
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var endpoint = $"{BaseEndpoint}/DeleteEstadoHabitacionById";
        return await _apiClient.DeleteAsync(endpoint, id);
    }

    public async Task<EstadoHabitacionModel> GetByDescripcionAsync(string descripcion)
    {
        var endpoint = $"{BaseEndpoint}/GetEstadoBy(descripcion){descripcion}";
        
        var result = await _apiClient.GetAsync<EstadoHabitacionModel>(endpoint);
        
        if (result == null || result.IdEstadoHabitacion <= 0)
        {
            return new EstadoHabitacionModel
            {
                IdEstadoHabitacion = -1,
                Descripcion = "Estado no disponible"
            };
        }
        
        return result;
    }
}