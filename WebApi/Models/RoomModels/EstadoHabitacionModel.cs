namespace WebApi.Models.RoomModels;

public class EstadoHabitacionModel : AuditEntity
{
    public int IdEstadoHabitacion { get; set; }
    public string Descripcion { get; set; }
}