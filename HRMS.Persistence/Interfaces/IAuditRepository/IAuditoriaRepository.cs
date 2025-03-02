using HRMS.Domain.Base;
using HRMS.Domain.Entities.Audit;
using HRMS.Domain.Repository;
namespace HRMS.Persistence.Interfaces.IAuditRepository
{
    public interface IAuditoriaRepository : IBaseRepository<Auditoria, int>
    {
        Task<OperationResult> LogAuditAsync(string accion);
        Task<List<Auditoria>> GetAuditByUserIdAsync(int idUsuario);
        Task<List<Auditoria>> GetAuditByDateTime(DateTime fechaRegistro);
    }
}

