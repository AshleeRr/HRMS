using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRMS.Domain.Entities.Users
{
    [Table("Usuario")]
    public class User : UserAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int IdUsuario { get; set; }
        public string? Clave { get; set; }
        [ForeignKey("UserRole")]
        public int? IdRolUsuario { get; set; } // FK

    }
}

