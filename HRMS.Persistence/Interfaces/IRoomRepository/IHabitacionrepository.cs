using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IRoomRepository;

public interface IHabitacionRepository : IBaseRepository<Habitacion, int>
{
    Task<OperationResult> GetByPisoAsync(int idPiso);
    
    Task<OperationResult> GetByNumeroAsync(string numero);
    
    Task<OperationResult> GetInfoHabitacionesAsync();
    
    Task<OperationResult> GetByCategoriaAsync(string categoria);
}
    