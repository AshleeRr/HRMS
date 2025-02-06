namespace HRMS.Domain.Base
{
    public abstract class AuditEntity
    {
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public bool Estatus { get; set; }
    }
}
