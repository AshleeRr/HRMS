using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using MyValidator.Validator;

public class PisoServiceValidator : Validator<PisoDto>
{
    public PisoServiceValidator()
    {
        AddRule(r => r.Descripcion != null).WithErrorMessage("La descripción no puede ser nula");
        AddRule(r => !string.IsNullOrEmpty(r.Descripcion)).WithErrorMessage("La descripción no puede estar vacía");
        AddRule(r => r.Descripcion == null || r.Descripcion.Length <= 50).WithErrorMessage("La descripción no puede tener más de 50 caracteres");
        AddRule(r => r.Descripcion != null && r.Descripcion.Length > 0).WithErrorMessage("La descripción es requerida");
    }
}