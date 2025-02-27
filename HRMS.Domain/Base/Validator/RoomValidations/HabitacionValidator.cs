namespace HRMS.Domain.Base.Validator.Reservation;

public class HabitacionValidator : Validator<Entities.RoomManagement.Habitacion>
{
   public HabitacionValidator()
   {
      AddRule(h => h!= null).WithErrorMessage( 
         "La habitación no puede ser nula");
      AddRule(h => h.IdPiso > 0).WithErrorMessage(
         "El piso de la habitación no puede ser nulo");
      AddRule(h => h.IdCategoria > 0).WithErrorMessage(
         "La categoría de la habitación no puede ser nula");
      AddRule(h => !string.IsNullOrEmpty(h.Numero)).WithErrorMessage(
         "El número de la habitación no puede ser nulo");
      AddRule(h => h.Detalle != null && h.Detalle.Length <= 100).WithErrorMessage(
         "El detalle de la habitación no puede exceder los 100 caracteres y no puede ser nulo");
      AddRule(h=> h.Precio > 0).WithErrorMessage(
         "El precio de la habitación tiene que ser mayor a 0");
      AddRule(h=> h.Estado != null).WithErrorMessage(
         "El estado de la habitación no puede ser nulo");
   } 
}