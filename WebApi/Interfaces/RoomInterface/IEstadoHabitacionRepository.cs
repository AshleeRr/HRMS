using WebApi.Models.RoomModels;

namespace WebApi.Interfaces.RoomInterface;

public interface IEstadoHabitacionRepository : IGenericInterface<EstadoHabitacionModel>
{
    Task<EstadoHabitacionModel> GetByDescripcionAsync(string descripcion);
}