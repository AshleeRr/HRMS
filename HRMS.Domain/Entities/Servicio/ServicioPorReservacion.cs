using HRMS.Domain.Entities.Reservations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Servicio
{
    [Table("ServiciosPorReservacion")]
    public class ServicioPorReservacion
    {
        public short ServicioID { get; set; }
        public int ReservacionID { get; set; }
        public decimal Precio { get; set; }
        public Reservation Reserva { get; set; }
        public Servicios Servicio  { get; set; }
    }
}
