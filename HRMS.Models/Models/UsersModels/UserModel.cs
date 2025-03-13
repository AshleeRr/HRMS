namespace HRMS.Models.Models.UsersModels
{
    public class UserModel 
    {
        public int IdUsuario { get; set; }
        public string? Email { get; set; }
        public int IdUserRol { get; set; }
        public string? UserRol { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
    }
}
