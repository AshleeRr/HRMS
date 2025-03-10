using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using Microsoft.IdentityModel.Tokens;
using MyValidator.Validator;

namespace HRMS.Application.DTOs.RoomManagementDto.Validations;

public class EstadoHabitacionServiceValidator : Validator<CreateEstadoHabitacionDto>
{
    public EstadoHabitacionServiceValidator()
    {
        AddRule(r => r.Descripcion.Length <= 50).WithErrorMessage("La descripción no puede exceder los 50 caracteres.");
        AddRule(r => r.Descripcion.IsNullOrEmpty()).WithErrorMessage("La descripción es requerida.");
        
    }
}