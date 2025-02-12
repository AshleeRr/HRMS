using HRMS.Domain.Base;


namespace HRMS.Domain.Entities.Reservation
{
    public class Reservation : AuditEntity
    {
        public int idRecepcion { get; set; }
        public int? idCliente { get; set; }
        public int? idHabitacion { get; set; }
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
    }
}
