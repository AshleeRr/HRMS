using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IRoomRepository;

public interface IPisoRepository : IBaseRepository<Piso, int>
{
    Task<OperationResult> GetByActivoAsync();
}