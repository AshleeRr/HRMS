namespace WebApi.Models.UsersModels.UserModels
{
    public class UserSaveModel
    {
        public string? NombreCompleto { get; set; }
        public string? Clave { get; set; }
        public int IdRolUsuario { get; set; }
        public string? Correo { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UserID { get; set; }
    }
}
