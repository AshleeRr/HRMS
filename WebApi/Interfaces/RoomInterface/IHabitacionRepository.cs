using WebApi.Models.RoomModels;

namespace WebApi.Interfaces.RoomInterface;

public interface IHabitacionRepository : IGenericInterface<HabitacionModel>
{
    /// <summary>
    /// Obtiene una habitación por su número
    /// </summary>
    Task<HabitacionModel> GetByNumeroAsync(string numero);
    /// <summary>
    /// Obtiene habitaciones por su categoría
    /// </summary>
    Task<IEnumerable<HabitacionModel>> GetByCategoriaAsync(string categoria);
    /// <summary>
    /// Obtiene habitaciones por su piso
    /// </summary>
    Task<IEnumerable<HabitacionModel>> GetByPisoAsync(int pisoId);
    
    /// <summary>
    /// Obtiene una lista de habitaciones con información adicional
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<HabitacionInfoModel>> GetInfoHabitaciones();
}