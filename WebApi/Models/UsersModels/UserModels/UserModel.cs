using HRMS.Application.DTOs.BaseDTO;

namespace WebApi.Models.UsersModels.UserModels
{
    public class UserModel : BaseUsersDTO
    {
        public int IdUsuario { get; set; }
        public int ReferenceID { get; set; }
        public int IdRolUsuario { get; set; }
    }
}
