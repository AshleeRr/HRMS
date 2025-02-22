using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Models.RoomManagementModel;

public class HabitacionUpdateDto : HabitacionCreateDto
{
    [Required] 
    public int IdHabitacion { get; set; }
}
