using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HRMS.Domain.Entities.Reservation
{

    [Table("Recepcion")]
    public class Reservation : AuditEntity
    {
        [Key]
        [Column("IdRecepcion")]
        public int IdRecepcion { get; set; }

        public DateTime? FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
        public DateTime? FechaSalidaConfirmacion { get; set; }

        public decimal? PrecioInicial { get; set; }
        public decimal? Adelanto { get; set; }

        public decimal? PrecioRestante { get; set; }

        public decimal TotalPagado { get; set; }
        public decimal? CostoPenalidad { get; set; }

        public string? Observacion
        {
            get; set;
        }

        [ForeignKey("idCliente")]
        public int? IdCliente { get; set; }

        [ForeignKey("idHabitacion")]
        public int? IdHabitacion { get; set; }
    }
}
