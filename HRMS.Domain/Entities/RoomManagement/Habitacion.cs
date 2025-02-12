using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

public sealed class Habitacion : AuditEntity
{
    public int Id { get; set; }
    public string? Numero { get; set;}
    public string? Detalle { get; set;}
    public decimal? Precio { get; set;}
    public int? IdEstadoHabitacion { get; set;}
    public int?IdPiso { get; set;}
    public int? IdCategoria { get; set;}
}