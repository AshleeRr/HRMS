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
        [ForeignKey("IdRolUsuario")]
        public int IdRolUsuario { get; set; }
        public int UserID { get; set; }
        public int ReferenceID { get; set; }
    }
}

