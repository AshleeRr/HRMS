namespace WebApi.Models.RoomModels;

public class HabitacionModel
{
    public int idHabitacion { get; set; }
    public string numero { get; set; }
    public string detalle { get; set; }
    public decimal precio { get; set; }
    public int idEstadoHabitacion { get; set; }
    public int idPiso { get; set; }
    public int idCategoria { get; set; }
    public DateTime changeTime { get; set; }
    public int userID { get; set; }
}