using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Users
{
    [Table("Cliente")]
    public class Client : UserAuditEntity
    {
        [Key]
        public int IdCliente { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }

        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }

        public Client(int idCliente, string? tipoDocumento = null, string? documento = null, string? nombreCompleto = null, string? correo = null, DateTime? fechaCreacion = null, bool? estado = null)
        {
            IdCliente = idCliente;
            TipoDocumento = tipoDocumento;
            Documento = documento;
            NombreCompleto = nombreCompleto;
            Correo = correo;
            FechaCreacion = fechaCreacion;
            Estado = estado;

        }
    }
}
