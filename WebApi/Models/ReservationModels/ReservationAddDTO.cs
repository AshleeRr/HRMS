using HRMS.WebApi.Models.BaseDTO;

namespace HRMS.WebApi.Models.Reservation_2023_0731
{
    public class ReservationAddDTO : DTOBase
    {
        public int RoomCategoryID { get; set; }
        public int PeopleNumber { get; set; }
        public decimal Adelanto { get; set; }
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
        public string Observations { get; set; }
        public List<int> Services { get; set; } = new List<int>();
    }
}
