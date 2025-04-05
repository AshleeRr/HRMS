

using HRMS.WebApi.Models.BaseDTO;

namespace HRMS.WebApi.Models.Reservation_2023_0731
{
    public class ReservationUpdateDTO : DTOBase
    {
        public int ID { get; set; }
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
        public string Observations { get; set; }
        public decimal AbonoPenalidad { get; set; }
    }
}
