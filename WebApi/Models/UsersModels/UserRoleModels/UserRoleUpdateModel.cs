namespace WebApi.Models.UsersModels.UserRoleModels
{
    public class UserRoleUpdateModel
    {
        public int IdRolUsuario { get; set; }

        public string RolNombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }
    }
}
