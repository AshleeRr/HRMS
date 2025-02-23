using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.RoomManagementModel;

public class PrecioUpdateDto
{
    [Range(0.01, 1000000)] 
    public decimal Precio { get; set; }
}