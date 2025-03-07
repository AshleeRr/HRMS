using MyValidator.Validator;


namespace HRMS.Application.DTOs.Reservation_2023_0731.ReservDtosValidator
{
    public class ReservUpdateDtoValidator : Validator<ReservationUpdateDTO>
    {
        public ReservUpdateDtoValidator()
        {
          //AddRule(r => r.In != null).WithErrorMessage("La fecha de entrada no puede ser nula");
          //AddRule(r => r.Out != null).WithErrorMessage("La fecha de salida no puede ser nula");

        }
    }
}
