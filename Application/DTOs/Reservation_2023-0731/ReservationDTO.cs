using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.DTOs.Reservation_2023_0731
{
    public class ReservationDTO : DTOBase
    {
        public int ReservationId { get; set; }
        public string State { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? DepartureConfirmationDate { get; set; }
        public decimal? InitialPrice { get; set; }
        public decimal? Advance { get; set; }
        public decimal? RemainingPrice { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal? PenaltyCost { get; set; }
        public string? Observation
        {
            get; set;
        }
        public int? ClientId { get; set; }
        public int? RoomId { get; set; }
    }
}
