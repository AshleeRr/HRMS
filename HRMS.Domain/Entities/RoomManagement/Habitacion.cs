using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

[Table("Habitacion")]
public sealed class Habitacion : AuditEntity
{
    [Key]
    [Column("IdHabitacion")]
    public int Id { get; set; }
    public string? Numero { get; set;}
    public string? Detalle { get; set;}
    public decimal? Precio { get; set; }
    
    [ForeignKey("EstadoHabitacion")]
    public int? IdEstadoHabitacion { get; set;}
    [ForeignKey("Piso")]
    public int?IdPiso { get; set;}
    [ForeignKey("Categoria")]
    public int? IdCategoria { get; set;}
}