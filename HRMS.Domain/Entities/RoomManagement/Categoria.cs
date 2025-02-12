using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

public  sealed class Categoria : AuditEntity
{
    public int Id { get; set; }
    string? Descripcion { get; set; }
    short IdServicio { get; set; }
}