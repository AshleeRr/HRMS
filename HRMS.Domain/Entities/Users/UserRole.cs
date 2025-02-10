using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Users
{
    [Table("RolUsuario")]
    public class UserRole : UserAuditEntity
    {
        [Key]
        public int IdRolUsuario { get; set; }
        public string? Description { get; set; }

        public UserRole(int idRolUsuario, string? description = null, DateTime? fechaCreacion = null, bool? estado = null)
        {
            IdRolUsuario = idRolUsuario;
            Description = description;
            FechaCreacion = fechaCreacion;
            Estado = estado;
        }
    }
}

