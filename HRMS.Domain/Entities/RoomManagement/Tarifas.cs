using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Domain.Base;

namespace HRMS.Domain.Entities.RoomManagement
{
    [Table("Tarifas")]
    public class Tarifas : AuditEntity
    {
        [Key]
        public int IdTarifa { get; set; } 

        [ForeignKey("IdHabitacion")] 
        public int? IdHabitacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        [Column(TypeName = "money")]
        public decimal PrecioPorNoche { get; set; }
        [Column(TypeName = "numeric(5,2)")]
        public decimal Descuento { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
