using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IRoomRepository;

public interface IHabitacionRepository : IBaseRepository<Habitacion, int>
{
    Task<OperationResult>GetByEstadoAsync(bool estado);
        
    Task<OperationResult> GetByPisoAsync(int idPiso);
    
    Task<OperationResult> GetByCategoriaAsync(int idCategoria);
    
    Task<OperationResult> GetByNumeroAsync(string numero);
    
}
    