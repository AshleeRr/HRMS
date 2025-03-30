using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IRoomRepository;

public interface ICategoryRepository : IBaseRepository<Categoria,int>
{
    Task<OperationResult> GetCategoriaByServiciosAsync(string nombre);
    Task<OperationResult> GetCategoriaByDescripcionAsync(string descripcion);
    Task<OperationResult> GetHabitacionByCapacidad(int capacidad);
}