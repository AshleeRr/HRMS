namespace WebApi.Models.RoomModels;

public class EstadoHabitacionModel
{
    public int IdEstadoHabitacion { get; set; }
    public string Descripcion { get; set; }
    public DateTime ChangeTime { get; set; }
    public int UserID { get; set; }
}