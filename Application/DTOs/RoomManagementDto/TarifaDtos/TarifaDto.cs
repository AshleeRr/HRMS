namespace HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;

public class TarifaDto : DTOBase
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public decimal Descuento { get; set; }
    public string Descripcion { get; set; }
}