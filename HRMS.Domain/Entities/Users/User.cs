using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Users
{
    [Table("Usuario")]
    public class User : UserAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }
        public string? Clave { get; set; }
        [ForeignKey("IdRolUsuario")]
        public int IdRolUsuario { get; set; }
        public string? Documento { get; set; }
        public string? TipoDocumento { get; set; }
    }
}

