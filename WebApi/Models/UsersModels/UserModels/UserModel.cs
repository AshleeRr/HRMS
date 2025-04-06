namespace WebApi.Models.UsersModels.UserModels
{
    public class UserModel
    {
        public int IdUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Clave { get; set; }
        public int IdRolUsuario { get; set; }
        public string? Correo { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public DateTime ChangeTime { get; set; }
        public int ReferenceID { get; set; }
        public int UserID { get; set; }
    }
}
