using HRMS.Domain.Base;

namespace HRMS.Domain.RoomManagement.Entities;

public sealed class Habitacion : AuditEntity
{
    public string? Numero { get; set;}
    public string? Detalle { get; set;}
    public decimal? Precio { get; set;}
    public int? IdEstadoHabitacion { get; set;}
    public int?IdPiso { get; set;}
    public int? IdCategoria { get; set;}
}