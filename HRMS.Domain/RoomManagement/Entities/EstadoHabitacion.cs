using HRMS.Domain.Base;

namespace HRMS.Domain.RoomManagement.Entities;

public sealed class EstadoHabitacion : AuditEntity
{
    string? Descripcion { get; set; }
}