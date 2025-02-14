using HRMS.Domain.Base;
using HRMS.Domain.Entities.Servicio;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IServicioRepository
{
    public interface IServicioRepository : IBaseRepository<Servicios, int>
    {
            Task<OperationResult> GetAllServiciosAsync();
            Task<OperationResult> GetServicioByIdAsync(short id);
            Task<OperationResult> AddServicioAsync(Servicios servicio);
            Task<OperationResult> UpdateServicioAsync(Servicios servicio);
            Task<OperationResult> DeleteServicioAsync(short id);
            Task<OperationResult> GetServiciosByNombreAsync(string nombre);
        
    }
    
}
