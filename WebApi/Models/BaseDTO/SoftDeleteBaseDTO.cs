using WebApi.Models.BaseDTO;

namespace HRMS.Application.DTOs.BaseDTO
{
    public class SoftDeleteBaseDTO : DtoBase
    {
        public bool Deleted { get; set; }
    }
}
