using HRMS.Application.DTOs.BaseDTO;

namespace WebApi.Models.UsersModels.UserModels
{
    public class UserSaveModel : BaseUsersDTO
    {
        public int IdRolUsuario { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
