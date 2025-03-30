using MyValidator.Validator;
using HRMS.Domain.Entities.RoomManagement;

namespace HRMS.Domain.Base.Validator.ServiceValidations;

public class TarifasValidator : Validator<Tarifas> 
{
    public TarifasValidator()
    {
        AddRule(t => t!= null).WithErrorMessage(
            "La tarifa no puede ser nula");
            
        AddRule(t => t.PrecioPorNoche > 0).WithErrorMessage(
            "El precio debe ser mayor a 0");
            
        AddRule(t => t.FechaInicio < t.FechaFin).WithErrorMessage(
            "La fecha de inicio debe ser menor a la fecha de fin");
        AddRule(t => t.Estado == false || t.FechaInicio > DateTime.Now).WithErrorMessage(
            "La fecha de inicio debe ser mayor a la fecha actual");
            
        AddRule(t => t.Descripcion.Length <= 255).WithErrorMessage(
            "La descripción no puede superar los 255 caracteres");
    }
}