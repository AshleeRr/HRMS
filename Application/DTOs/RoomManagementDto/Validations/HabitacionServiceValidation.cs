using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using MyValidator.Validator;

namespace HRMS.Application.DTOs.RoomManagementDto.Validations;

public class HabitacionServiceValidation : Validator<CreateHabitacionDTo>
{
    public HabitacionServiceValidation()
    {
        AddRule(h => !string.IsNullOrEmpty(h.Numero))
            .WithErrorMessage("El número de la habitación no puede ser nulo o vacío");
            
        AddRule(h => !(h.Numero != null && h.Numero.Length > 3))
            .WithErrorMessage("El número de la habitación no puede exceder los 3 caracteres");
        
        AddRule(h => !string.IsNullOrEmpty(h.Detalle))
            .WithErrorMessage("El detalle de la habitación no puede ser nula o vacía");
            
        AddRule(h => !(h.Detalle != null && h.Detalle.Length > 100))
            .WithErrorMessage("El detalle de la habitación no puede exceder los 100 caracteres");
        
        AddRule(h => !(h.Precio == null || h.Precio <= 0))
            .WithErrorMessage("El precio de habitación no puede ser cero o negativo");
        
        AddRule(h => !(h.IdPiso == null || h.IdPiso <= 0))
            .WithErrorMessage("El piso de la habitación no puede ser cero o negativo");
        
        AddRule(h => !(h.IdCategoria == null || h.IdCategoria <= 0))
            .WithErrorMessage("El id de la categoria no puede ser cero o negativo");
        
        AddRule(h => !(h.IdEstadoHabitacion == null || h.IdEstadoHabitacion <= 0))
            .WithErrorMessage("El estado de la habitación no puede ser cero o negativo");
    }
}