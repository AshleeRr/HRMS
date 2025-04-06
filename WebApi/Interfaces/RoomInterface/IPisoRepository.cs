using WebApi.Models.RoomModels;

namespace WebApi.Interfaces.RoomInterface;

public interface IPisoRepository : IGenericInterface<PisoModel>
{
    Task<PisoModel> GetByDescripcionAsync(string descripcion);
}