namespace WebApi.Models.UsersModels.UserModels
{
    public class UserSaveModel
    {
        public int IdUserRole { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public DateTime ChangeTime { get; set; }
        public int UserID { get; set; }
    }
}
