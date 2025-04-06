using HRMS.WebApi.Models.BaseDTO;

namespace WebApi.Models.UsersModels.UserRoleModels
{
    public class UserRoleSaveModel : DTOBase
    {
        public string? RolNombre { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
