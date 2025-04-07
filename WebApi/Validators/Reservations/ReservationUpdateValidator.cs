using HRMS.WebApi.Models.Reservation_2023_0731;
using MyValidator.Validator;

namespace WebApi.Validators.Reservations
{
    public class ReservationUpdateValidator : Validator<ReservationUpdateDTO>
    {
        public ReservationUpdateValidator()
        {
            AddRule(r => r.ID > 0)
                .WithErrorMessage("El ID de la reserva debe ser mayor que cero");

            AddRule(r => r.In > DateTime.MinValue)
                .WithErrorMessage("La fecha de entrada debe ser válida");

            AddRule(r => r.Out > r.In)
                .WithErrorMessage("La fecha de salida debe ser posterior a la de entrada");

            AddRule(r => !string.IsNullOrWhiteSpace(r.Observations))
                .WithErrorMessage("Debe ingresar una observación");

            AddRule(r => r.AbonoPenalidad >= 0)
                .WithErrorMessage("El abono a la penalidad no puede ser negativo");
        }

      
    }
}
