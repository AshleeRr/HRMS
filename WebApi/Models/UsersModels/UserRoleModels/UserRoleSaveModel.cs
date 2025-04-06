namespace WebApi.Models.UsersModels.UserRoleModels
{
    public class UserRoleSaveModel
    {
        public string? RolNombre { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UserID { get; set; }
    }
}