using WebApi.Models.BaseDTO;

namespace WebApi.Models.RoomModels;

public class EstadoHabitacionModel : DtoBase
{
    public int IdEstadoHabitacion { get; set; }
    public string Descripcion { get; set; }
}