using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.RoomValidations;

public class CategoriaValidator : Validator<Entities.RoomManagement.Categoria>
{
    public CategoriaValidator()
    {
        AddRule(c => c !=null).WithErrorMessage("La categoria no puede ser nula");
        AddRule(c => c.Descripcion != null && c.Descripcion.Length <= 50).WithErrorMessage(
            "La description de la categoria debe tener menos de 50 caracteres");
        AddRule(c => c.Capacidad > 1).WithErrorMessage(
            "La capacidad de la categoria debe ser mayor a 1");
    }
}