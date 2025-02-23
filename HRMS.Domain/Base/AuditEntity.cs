
namespace HRMS.Domain.Base
{
    public abstract class AuditEntity
    {
        public DateTime? FechaCreacion { get; set; }
        public bool? Estado { get; set; } = true;
    }
}