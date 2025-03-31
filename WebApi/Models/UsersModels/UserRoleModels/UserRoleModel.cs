using System.Text.Json.Serialization;

namespace WebApi.Models.UsersModels.UserRoleModels
{
    public class UserRoleModel
    {
        public int IdUserRole { get; set; }

        public string Descripcion { get; set; }

        public string Nombre { get; set; }

        public DateTime ChangeTime { get; set; }

        public int UserID { get; set; }
    }
}
