namespace HRMS.Application.DTOs.RoomManagementDto.HabitacionDto;

public class CreateHabitacionDTo : DTOBase
{
    public string Numero { get; set; }
    public string Detalle { get; set; }
    public decimal Precio { get; set; }
    public int IdEstadoHabitacion { get; set; }
    public int IdPiso { get; set; }
    public int IdCategoria { get; set; }   
}