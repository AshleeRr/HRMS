namespace HRMS.Models.Models.UsersModels
{
    public class UserModel : PersonModel
    {
        public int IdUsuario { get; set; }
        public string? Email { get; set; }
        public int IdUserRol { get; set; }
        public string? UserRol { get; set; }

    }
}
