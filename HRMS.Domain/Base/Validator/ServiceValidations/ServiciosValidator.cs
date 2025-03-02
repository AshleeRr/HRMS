using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.ServiceValidations;

public class ServiciosValidator : Validator<Entities.Servicio.Servicios>
{
    public ServiciosValidator()
    {
        AddRule(s => s != null).WithErrorMessage("El servicio no puede ser nulo");
        AddRule(s => s.Nombre.Length<=200).WithErrorMessage(
            "El nombre no puede superar los 200 caracteres");
    }
}