using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.RoomValidations;

public class PisoValidator : Validator<Entities.RoomManagement.Piso>
{
    public PisoValidator()
    {
        AddRule(p => p != null).WithErrorMessage("El piso no puede ser nulo");
        AddRule(p=> p.Descripcion.Length <=50).WithErrorMessage( 
         "La descripción no puede superar los 50 caracteres");   
    }
}