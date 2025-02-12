
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

[Table("Servicios")]
public sealed class Servicios : AuditEntity
{
    [Key]
    public short Id { get; set; }
    public string Nombre { get; set; } = String.Empty;
    public string Descripcion { get; set; } = String.Empty;
}