using HRMS.Application.DTOs.BaseDTO;

namespace HRMS.Application.DTOs.UserRoleDTOs
{
    public class RemoveUserRoleDTO : SoftDeleteBaseDTO
    {
        public int IdUserRole { get; set; }
    }
}
