using HRMS.Domain.Base;
using HRMS.Domain.Entities.Servicio;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IServicioRepository
{
    public interface IServicioRepository : IBaseRepository<Servicios, short>
    { 
        Task<OperationResult> GetServiciosByNombreAsync(string nombre);
    }
}
