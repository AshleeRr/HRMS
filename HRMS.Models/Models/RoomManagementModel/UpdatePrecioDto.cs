using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Models.RoomManagementModel;

public class PrecioUpdateDto
{
    [Range(0.01, 1000000)] 
    public decimal Precio { get; set; }
}