using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMS.Application.DTOs.BaseDTO;

namespace HRMS.Application.DTOs.Reservation_2023_0731
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
