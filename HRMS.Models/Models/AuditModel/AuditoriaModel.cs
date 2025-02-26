
namespace HRMS.Models.Models.AuditModel
{
    public class AuditoriaModel
    {
        public int IdAuditoria { get; set; }
        public string Accion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuario { get; set; }
    }
}
