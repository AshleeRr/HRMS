using HRMS.WebApi.Models.BaseDTO;

namespace HRMS.Application.DTOs.BaseDTO
{
    public class SoftDeleteBaseDTO : DTOBase
    {
        public bool Deleted { get; set; }
    }
}
