using HRMS.WebApi.Models.BaseDTO;

namespace HRMS.WebApi.Models.Reservation_2023_0731
{
    public class ReservClientInfoDTO : DTOBase
    {
        public int ReservationID { get; set; }
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
        public decimal Total { get; set; }
        public string? RoomNumber { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
    }
}
