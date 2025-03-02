using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.ServiceValidations;

public class TarifasValidator : Validator<Entities.RoomManagement.Tarifas>
{
    public TarifasValidator()
    {
        AddRule(t => t!= null).WithErrorMessage(
            "La tarifa no puede ser nula");
        AddRule(t=> t.PrecioPorNoche > 0 && IsInteger(t.PrecioPorNoche)).WithErrorMessage(
            "El precio debe ser mayor y ser un numero entero");
        AddRule(t=> t.FechaInicio < t.FechaFin).WithErrorMessage(
            "La fecha de inicio debe ser menor a la fecha de fin");
        AddRule(t=> t.FechaInicio > DateTime.Now).WithErrorMessage(
            "La fecha de inicio debe ser mayor a la fecha actual");
        AddRule(t => t.Descripcion.Length <= 255).WithErrorMessage(
            "La descripción no puede superar los 255 caracteres");
    }
    
    
    private bool IsInteger(decimal? num)
    {
        if (!num.HasValue)
            return false;
        var numero = num.Value;
        return num % 1 == 0;
    }
}
