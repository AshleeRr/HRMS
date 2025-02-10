namespace HRMS.Domain.Base
{
    public abstract class UserAuditEntity
    {
        public DateTime? FechaCreacion { get; set; }
        public bool? Estado { get; set; }
    }
}
