using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

public sealed class Piso : AuditEntity
{
    public int Id { get; set; }
    public string? Descripcion{ get; set; }
}