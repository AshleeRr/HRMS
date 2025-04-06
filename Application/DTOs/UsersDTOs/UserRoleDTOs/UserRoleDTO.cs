using HRMS.Application.DTOs.BaseDTO;

namespace HRMS.Application.DTOs.UsersDTOs.UserRoleDTOs
{
    public class UserRoleDTO : DTOBase
    {
        public string? RolNombre { get; set; }
        public string? Descripcion { get; set; }
       
    }
}
