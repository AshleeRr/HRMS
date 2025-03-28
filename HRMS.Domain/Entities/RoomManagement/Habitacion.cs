﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

[Table("Habitacion")]
public class Habitacion : AuditEntity
{ 
    [Key]
    public int IdHabitacion { get; set; }
    public string? Numero { get; set;}
    public string? Detalle { get; set;}
    [Column(TypeName = "decimal(10,2)")]
    public decimal? Precio { get; set; }
    
    [ForeignKey("EstadoHabitacion")]
    public int? IdEstadoHabitacion { get; set;}
    [ForeignKey("Piso")]
    public int?IdPiso { get; set;}
    [ForeignKey("Categoria")]
    public int? IdCategoria { get; set;}
}