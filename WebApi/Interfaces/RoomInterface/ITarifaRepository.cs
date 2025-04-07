using WebApi.Models.RoomModels;

namespace WebApi.Interfaces.RoomInterface;

public interface ITarifaRepository : IGenericInterface<TarifaModel>
{
    Task<IEnumerable<TarifaModel>> GetTarifaByPrecio(decimal precio);
    Task<IEnumerable<TarifaModel>> GetTarifaByFecha(DateTime fecha);
}