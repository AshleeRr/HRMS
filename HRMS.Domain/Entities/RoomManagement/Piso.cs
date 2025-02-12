using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

[Table("Piso")]
public sealed class Piso : AuditEntity
{
    public int Id { get; set; }
    public string? Descripcion{ get; set; }
}