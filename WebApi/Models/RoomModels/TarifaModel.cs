namespace WebApi.Models.RoomModels;

public class TarifaModel : AuditEntity
{
    public int IdTarifa { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public decimal Descuento { get; set; }
    public string Descripcion { get; set; }
    public int IdCategoria { get; set; }
    
}