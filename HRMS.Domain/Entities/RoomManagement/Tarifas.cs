using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Domain.Entities.RoomManagement
{
    [Table("Tarifas")]
    public class Tarifas : AuditEntity
    {
        [Key] 
        public int IdTarifa { get; set; }

        /*[ForeignKey("IdHabitacion")] 
        public int IdHabitacion { get; set; }*/
        public int IdCategoria { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        [Column(TypeName = "money")] 
        public decimal PrecioPorNoche { get; set; }

        [Column(TypeName = "numeric(5,2)")] 
        public decimal Descuento { get; set; }

        public string Descripcion { get; set; } = string.Empty;
    }
}
