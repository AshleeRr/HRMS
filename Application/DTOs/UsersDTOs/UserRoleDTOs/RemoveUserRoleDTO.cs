using HRMS.Application.DTOs.BaseDTO;

namespace HRMS.Application.DTOs.UsersDTOs.UserRoleDTOs
{
    public class RemoveUserRoleDTO : SoftDeleteBaseDTO
    {
        public int IdUserRole { get; set; }
    }
}
