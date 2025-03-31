namespace WebApi.Models.UsersModels.UserRoleModels
{
    public class UserRoleSaveModel
    {
        public string Descripcion { get; set; }

        public string Nombre { get; set; }

        public DateTime ChangeTime { get; set; }

        public int UserID { get; set; }
    }
}
