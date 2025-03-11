namespace HRMS.Application.DTOs.UserDTOs
{
    public class UserDTO : DTOBase
    {
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public int IdUserRole { get; set; }
        public string Clave { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
    }
}
