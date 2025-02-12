namespace HRMS.Domain.Base
{
    public abstract class UserAuditEntity : AuditEntity
    {

        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
    }
}