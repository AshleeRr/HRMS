namespace WebApi.Models.UsersModels.UserRoleModels
{
    public class UserRoleModel
    {
        public int IdRolUsuario { get; set; }
        public string? RolNombre { get; set; }
        public string? Descripcion { get; set; }
        public DateTime ChangeTime { get; set; }
        public int UserID { get; set; }
    }
}
