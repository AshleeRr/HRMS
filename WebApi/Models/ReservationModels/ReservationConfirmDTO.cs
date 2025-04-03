using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMS.WebApi.Models.BaseDTO;

namespace HRMS.WebApi.Models.Reservation_2023_0731
{
    public class ReservationConfirmDTO : DTOBase
    {
        public int ReservationId { get; set; }
        public decimal Abono { get; set; }
    }
}
