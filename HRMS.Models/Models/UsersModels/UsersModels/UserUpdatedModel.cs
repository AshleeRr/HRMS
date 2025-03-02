namespace HRMS.Models.Models.UsersModels.UsersModels
{
    public class UserUpdatedModel : PersonModel
    {
        public bool Estado { get; set; }
        public string Clave { get; set; }
        public int IdUserRole { get; set; }
    }
}
