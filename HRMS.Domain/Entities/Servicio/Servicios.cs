using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Domain.Base;


namespace HRMS.Domain.Entities.Servicio
{
    [Table("Servicios")]
    public sealed class Servicios : AuditEntity
    {
        [Key]
        public short IdServicio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        
    }
}
