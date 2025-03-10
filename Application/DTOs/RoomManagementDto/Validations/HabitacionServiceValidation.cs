using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using Microsoft.IdentityModel.Tokens;
using MyValidator.Validator;

namespace HRMS.Application.DTOs.RoomManagementDto.Validations;

public class HabitacionServiceValidation : Validator<CreateHabitacionDTo>
{
    public HabitacionServiceValidation()
    {
        AddRule( h => h.Numero.IsNullOrEmpty()).WithErrorMessage("El número de la habitación no puede ser nulo o vacío");
        AddRule( h => h.Numero.Length >= 3).WithErrorMessage("El número de la habitación no puede exceder los 3 caracteres");
        AddRule(h => h.IdPiso <= 0).WithErrorMessage("El piso de la habitación no puede ser cero o negativo");
        AddRule(h => h.IdCategoria <= 0).WithErrorMessage("El id de la categoria no puede ser cero o negativo");
        AddRule(h => h.IdEstadoHabitacion <= 0).WithErrorMessage("El estado de la habitación no puede ser cero o negativo");
        AddRule(h=> h.Precio <= 0).WithErrorMessage("El precio de habitación no puede ser cero o negativo");
        AddRule(h => h.Detalle.IsNullOrEmpty()).WithErrorMessage("El detalle de la habitación no puede ser nula o vacía");
        AddRule(h => h.Detalle.Length >= 100).WithErrorMessage("El detalle de la habitación no puede exceder los 100 caracteres");
    }
}