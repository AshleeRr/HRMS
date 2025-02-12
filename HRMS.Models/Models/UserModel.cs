namespace HRMS.Models.Models
{
    public class UserModel : PersonModel
    {
        public int IdUsuario { get; set; }
       
        public string? Clave { get; set; }
        public int IdUserRol { get; set; }

    }
}
