using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

public sealed class EstadoHabitacion : AuditEntity
{
    public int Id { get; set; }
    string? Descripcion { get; set; }
}