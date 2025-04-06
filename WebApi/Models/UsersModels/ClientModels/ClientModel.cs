using HRMS.Application.DTOs.BaseDTO;

namespace WebApi.Models.UsersModels.ClientModels
{
    public class ClientModel : BaseUsersDTO
    {
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
    }
}
