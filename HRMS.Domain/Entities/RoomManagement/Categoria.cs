﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement;

[Table("Categoria")]
public  sealed class Categoria : AuditEntity
{
    [Key]
    public int Id { get; set; }
    public string? Descripcion { get; set; }
    
    [ForeignKey("Servicio")]
    public short IdServicio { get; set; }
}