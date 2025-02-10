using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Users
{
    [Table("Usuario")]
    public class Users : UserAuditEntity
    {
        [Key]
        public int IdUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        [ForeignKey("UserRole")]
        public int? IdRolUsuario { get; set; } // FK
        
        public Users(int idUsuario, string? nombreCompleto = null, string? correo = null, string? clave = null, int? idRolUsuario = null, DateTime? fechaCreacion = null, bool? estado = null)
        {
            IdUsuario = idUsuario;
            NombreCompleto = nombreCompleto;
            Correo = correo;
            Clave = clave;
            IdRolUsuario = idRolUsuario;
            FechaCreacion = fechaCreacion;
            Estado = estado;
        }
    }
}

