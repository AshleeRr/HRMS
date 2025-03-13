using HRMS.Application.DTOs.UserDTOs;

namespace HRMS.Application.DTOs.ClientDTOs
{
    public class SaveClientDTO : SaveUserClientDTO
    {
        public int IdUsuario { get; set; }
    }
}
