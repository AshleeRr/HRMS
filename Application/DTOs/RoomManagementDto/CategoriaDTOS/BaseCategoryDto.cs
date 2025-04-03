using HRMS.Application.DTOs.BaseDTO;

namespace HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS
{
    public abstract class BaseCategoryDto : DTOBase
    {
        public string? Descripcion { get; set; }
        public short IdServicio { get; set; }
        public int Capacidad { get; set; }
    }
}

