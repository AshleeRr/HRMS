
namespace WebApi.Models.RoomModels;

public class HabitacionInfoModel
{
    public int IdHabitacion { get; set; }
    public string Numero { get; set; }
    public string Detalle { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public string DescripcionPiso { get; set; }
    public string DescripcionCategoria { get; set; }
    public string NombreServicio { get; set; }
    public string DescripcionServicio { get; set; }
   
}