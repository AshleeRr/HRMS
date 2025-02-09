using HRMS.Domain.Base;

namespace HRMS.Domain.RoomManagement.Entities;

public  sealed class Categoria : AuditEntity
{
    string? Descripcion { get; set; }
    short IdServicio { get; set; }
}