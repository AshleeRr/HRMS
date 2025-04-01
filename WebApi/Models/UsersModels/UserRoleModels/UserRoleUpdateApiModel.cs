namespace WebApi.Models.UsersModels.UserRoleModels
{
    public class UserRoleUpdateApiModel
    {
        public int IdUserRole { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }
    }
}
