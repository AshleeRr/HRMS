using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.RoomManagementModel;

public class HabitacionCreateDto
{
    [Required] 
    public string Numero { get; set; }
    [Required] 
    public string Detalle { get; set; }
    [Range(0.01, 1000000)]
    public decimal Precio { get; set; }
    [Range(1, int.MaxValue)] 
    public int IdEstadoHabitacion { get; set; }
    [Range(1, int.MaxValue)] 
    public int IdPiso { get; set; }
    [Range(1, int.MaxValue)] 
    public int IdCategoria { get; set; }
}