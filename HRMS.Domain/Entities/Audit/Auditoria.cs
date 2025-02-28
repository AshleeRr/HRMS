using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Audit
{
    public class Auditoria
    {
        [Key]
        public int IdAuditoria { get; set; }
        public string Accion { get; set; }
        public DateTime FechaRegistro { get; set; }
        [ForeignKey ("IdUsuario")]
        public int IdUsuario { get; set; }
    }
}
