using HRMS.WebApi.Models.Reservation_2023_0731;
using MyValidator.Validator;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Validators.Reservations
{
    public class ReservationCreateValidator : Validator<ReservationAddDTO>
    {

        public ReservationCreateValidator()
        {
            AddRule(r => r.RoomCategoryID > 0)
                .WithErrorMessage("El id debe ser mayor que cero");
            AddRule(r => r.PeopleNumber > 0)
               .WithErrorMessage("El número de personas debe ser mayor que cero");

            AddRule(r => r.Adelanto >= 0)
                .WithErrorMessage("El adelanto no puede ser negativo");

            AddRule(r => r.In > DateTime.MinValue)
                .WithErrorMessage("La fecha de entrada debe ser válida");

            AddRule(r => r.Out > r.In)
                .WithErrorMessage("La fecha de salida debe ser posterior a la fecha de entrada");

            AddRule(r => !string.IsNullOrWhiteSpace(r.Observations))
                .WithErrorMessage("Debe ingresar una observación");
        }
    }
}
