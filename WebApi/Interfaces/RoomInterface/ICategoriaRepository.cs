using WebApi.Models.RoomModels;

namespace WebApi.Interfaces.RoomInterface;

public interface ICategoriaRepository : IGenericInterface<CategoriaModel>
{
    /// <summary>
    /// Obtiene una categoría por su descripción
    /// </summary>
    Task<CategoriaModel> GetByDescripcionAsync(string descripcion);
        
    /// <summary>
    /// Obtiene categorías por capacidad
    /// </summary>
    Task<IEnumerable<CategoriaModel>> GetByCapacidadAsync(int capacidad);
}