using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Users
{
    [Table("RolUsuario")]
    public class UserRole : AuditEntity
    {
        [Key]
        public int IdRolUsuario { get; set; }
        public string? Descripcion { get; set; }

        public UserRole(int idRolUsuario, string? descripcion = null)
        {
            IdRolUsuario = idRolUsuario;
            Descripcion = descripcion;

        }
    }
}