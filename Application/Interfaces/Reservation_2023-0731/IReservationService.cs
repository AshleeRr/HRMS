using HRMS.Application.Base;
using HRMS.Application.DTOs.Reservation_2023_0731;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.Reservation_2023_0731
{
    public interface IReservationService : IBaseServices<ReservationAddDTO, ReservationUpdateDTO, ReservationRemoveDTO>
    {
        Task<OperationResult> ConfirmReservation(int id);
        Task<OperationResult> CancelReservation(int id);
        Task<OperationResult> GetReservationByClientID(int id);
    }
}
