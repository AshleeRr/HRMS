using HRMS.WebApi.Models.BaseDTO;

namespace WebApi.Models.UsersModels
{
    public class UserRoleModel : DTOBase
    {
        public int IdRolUsuario { get; set; }
        public string? RolNombre { get; set; }
        public string? Descripcion { get; set; }
    }
}
