using HRMS.Domain.Base;
namespace HRMS.Persistence.Interfaces.IAuditRepository
{
    public interface IAuditoriaRepository
    {
        Task<OperationResult> LogAuditAsync(string accion, int IdUsuario);
        Task<OperationResult> GetAuditByUserIdAsync(int IdUsuario);
        Task<OperationResult> GetAuditByDateTime(DateTime FechaRegistro);
    }
}

