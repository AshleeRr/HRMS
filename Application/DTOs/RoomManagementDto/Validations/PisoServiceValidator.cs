using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using MyValidator.Validator;

namespace HRMS.Application.DTOs.RoomManagementDto.Validations;

public class PisoServiceValidator : Validator<CreatePisoDto>
{
    public PisoServiceValidator()
    {
        AddRule(r => r.Descripcion.Length <= 50).WithErrorMessage("La descripción no puede tener más de 50 caracteres");
        AddRule(r => r.Descripcion.Length > 0).WithErrorMessage("La descripción es requerida");
        
    }
}