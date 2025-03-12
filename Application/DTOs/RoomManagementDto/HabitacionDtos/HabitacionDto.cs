namespace HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;

public class HabitacionDto : DTOBase
{
    public int IdHabitacion { get; set; }
    public string Numero { get; set; }
    public string Detalle { get; set; }
    public decimal? Precio { get; set; }
    public int ?IdEstadoHabitacion { get; set; }
    public int ?IdPiso { get; set; }
    public int ?IdCategoria { get; set; } 
}