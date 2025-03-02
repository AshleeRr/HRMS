using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Models.Models.ReservationModels
{
    public class CategoryRoomForReserv
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal Descuento { get; set; }

    }
}
