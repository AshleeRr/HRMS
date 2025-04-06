
using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Users
{
    [Table("Cliente")]
    public class Client : UserAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCliente { get; set; }
        [ForeignKey("IdUsuario")]
        public int IdUsuario { get; set; }
        public int UserID { get; set; }
    }
}
