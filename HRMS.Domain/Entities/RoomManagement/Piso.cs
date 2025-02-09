using HRMS.Domain.Base;

namespace HRMS.Domain.RoomManagement.Entities;

public sealed class Piso : AuditEntity
{
    public string? Descripcion{ get; set; }
}