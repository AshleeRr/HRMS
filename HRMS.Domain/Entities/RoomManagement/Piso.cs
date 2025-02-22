using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

[Table("Piso")]
public sealed class Piso : AuditEntity
{
    [Key]
    public int IdPiso { get; set; }
    public string? Descripcion{ get; set; }
}