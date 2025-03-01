using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.RoomValidations;
public class HabitacionValidator : Validator<Entities.RoomManagement.Habitacion>
{ 
   public HabitacionValidator()
   {
      AddRule(h => h != null).WithErrorMessage(
         "La habitación no puede ser nula");
      AddRule(h => h.IdPiso > 0).WithErrorMessage(
         "El piso de la habitación no puede ser nulo o inválido");
      AddRule(h => h.IdCategoria > 0).WithErrorMessage(
         "La categoría de la habitación no puede ser nula o inválida");
      AddRule(h => !string.IsNullOrWhiteSpace(h.Numero)).WithErrorMessage(
         "El número de la habitación no puede ser nulo o vacío");
      AddRule(h => h.Numero == null || h.Numero.Length <= 10).WithErrorMessage(
         "El número de la habitación no puede exceder los 10 caracteres");
      AddRule(h => h.Detalle != null && h.Detalle.Length <= 100).WithErrorMessage(
         "El detalle de la habitación no puede exceder los 100 caracteres");
      AddRule(h => h.Precio > 0 && h.Precio <= 100000).WithErrorMessage(
         "El precio de la habitación debe estar entre 1 y 100,000");
      AddRule(h => h.IdEstadoHabitacion > 0).WithErrorMessage(
         "El estado de la habitación no puede ser nulo o inválido");
   } 
}