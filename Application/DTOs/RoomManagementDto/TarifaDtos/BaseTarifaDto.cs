using HRMS.Application.DTOs.BaseDTO;

namespace HRMS.Application.DTOs.RoomManagementDto.TarifaDtos
{
    public class BaseTarifaDto : DTOBase
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioPorNoche { get; set; }
        public decimal Descuento { get; set; }
        public string Descripcion { get; set; }
    
        public int IdCategoria { get; set; }
    }  
}

