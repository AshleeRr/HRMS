using System.ComponentModel.DataAnnotations;
using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace HRMS.Domain.Entities.RoomManagement;

[Table("EstadoHabitacion")]
public  class EstadoHabitacion : AuditEntity
{
    [Key]
    public int IdEstadoHabitacion { get; set; }
    public string? Descripcion { get; set; }
}