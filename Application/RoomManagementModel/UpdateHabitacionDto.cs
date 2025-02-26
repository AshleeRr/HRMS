using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.RoomManagementModel;

public class HabitacionUpdateDto : HabitacionCreateDto
{
    [Required] 
    public int IdHabitacion { get; set; }
}
