namespace HRMS.Application.DTOs.RoomManagementDto.TarifaDto;

public class CreateTarifaDto : DTOBase
{
    public int IdCategoria { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public decimal Descuento { get; set; }
    public string Descripcion { get; set; } = String.Empty;
}