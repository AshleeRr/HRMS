using HRMS.Application.DTOs.BaseDTO;

namespace HRMS.Application.DTOs.UserDTOs
{
    public class RemoveUserClientDTO : SoftDeleteBaseDTO
    {
        public int Id { get; set; } 
    }
}
