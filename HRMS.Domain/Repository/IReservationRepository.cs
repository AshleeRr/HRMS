using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservations;

namespace HRMS.Domain.Repository
{
    public interface IReservationRepository : IBaseRepository<Reservation, int>
    {
        Task<OperationResult> GetReservationsByClientId(int clientId);

        Task<OperationResult> GetDisponibleRoomsOfCategoryInTimeLapse(DateTime start, DateTime end, int categoriaId);

        Task<OperationResult> GetPricesForServicesinRoomCategory(int categoryId, IEnumerable<int> servicesIds);
        Task<bool> HasRoomCapacity(int categoryId, int people);
        Task<OperationResult> GetCategoryForReserv(int categoryId, int people, DateTime start, DateTime end);

    }
}
