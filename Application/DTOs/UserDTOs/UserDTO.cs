using HRMS.Application.DTOs.BaseDTO;

namespace HRMS.Application.DTOs.UserDTOs
{
    public class UserDTO : DTOBase
    {
        public int IdUserRole { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
    }
}
