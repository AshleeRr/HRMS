namespace HRMS.Domain.Base
{
    public abstract class UserAuditEntity : AuditEntity
    {
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? Clave { get; set; }

    }
}