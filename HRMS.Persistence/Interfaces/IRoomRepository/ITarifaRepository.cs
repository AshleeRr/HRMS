using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IRoomRepository;

public interface ITarifaRepository : IBaseRepository<Tarifas, int>
{
    Task<OperationResult> GetTarifasVigentesAsync(DateTime fecha);
    Task<OperationResult> GetTarifasPorHabitacionAsync(int idHabitacion);
    Task<OperationResult>GetTarifaActivaByHabitacionAsync(int idHabitacion);
}