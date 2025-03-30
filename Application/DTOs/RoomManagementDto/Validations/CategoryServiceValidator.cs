using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using Microsoft.IdentityModel.Tokens;
using MyValidator.Validator;

namespace HRMS.Application.DTOs.RoomManagementDto.Validations;

public class CategoryServiceValidator : Validator<CreateCategoriaDto>
{
    public CategoryServiceValidator()
    {
        
        AddRule(c => c.Descripcion != null && !c.Descripcion.IsNullOrEmpty())
            .WithErrorMessage("La descripcion no puede estar vacia");
        
        AddRule(c => c.Descripcion == null || c.Descripcion.Length <= 50)
            .WithErrorMessage("La descripcion categoria no puede superar los 50 caracteres");
        
        AddRule(c => c.Descripcion == null || c.Descripcion.Length >= 3)
            .WithErrorMessage("La descripcion categoria debe tener al menos 3 caracteres");
        
        AddRule(c => c.IdServicio > 0)
            .WithErrorMessage("El id del servicio debe ser mayor a 0");
        AddRule(c => c.Capacidad > 0)
            .WithErrorMessage("la capacidad debe ser mayor a 0");
    }
}