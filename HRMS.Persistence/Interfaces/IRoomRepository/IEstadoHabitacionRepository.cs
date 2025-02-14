using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IRoomRepository;

public interface IEstadoHabitacionRepository : IBaseRepository<EstadoHabitacion, int>
{
    Task<OperationResult> GetActivosAsync();
}