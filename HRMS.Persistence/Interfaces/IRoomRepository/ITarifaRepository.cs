using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IRoomRepository;

public interface ITarifaRepository : IBaseRepository<Tarifas, int>
{ 
    Task<OperationResult> GetTarifasVigentesAsync(String fechaInput);
    Task<OperationResult>GetHabitacionByPrecioAsync(decimal precio);
}