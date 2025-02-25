using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRMS.Domain.Entities.Users
{
    [Table("RolUsuario")]
    public class UserRole : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int IdRolUsuario { get; set; }
        public string? Descripcion { get; set; }
    }
}