using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservation;

namespace HRMS.Domain.Repository
{
    public interface IReservationRepository : IBaseRepository<Reservation, int>
    {
        Task<OperationResult> GetReservationsByClientId(int clientId);
    }
}
