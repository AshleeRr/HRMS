using MyValidator.Validator;


namespace HRMS.Application.DTOs.Reservation_2023_0731.ReservDtosValidator
{
    public class ReservAddDtoValidator : Validator<ReservationAddDTO>
    {
        public ReservAddDtoValidator()
        {
            //AddRule(r => r.In != null).WithErrorMessage("La fecha de entrada no puede ser nula");
            //AddRule(r => r.Out != null).WithErrorMessage("La fecha de salida no puede ser nula");
            AddRule(r => r.Adelanto != 0).WithErrorMessage("El adelanto no puede ser cero");
            AddRule(r => r.Observations != null).WithErrorMessage("Las observaciones no pueden ser nulas");
            AddRule(r => r.PeopleNumber != 0).WithErrorMessage("El número de personas no puede ser cero");
            AddRule(r => r.RoomCategoryID != 0).WithErrorMessage("El ID de la categoría de la habitación no puede ser cero");
            AddRule(r => r.UserID != 0).WithErrorMessage("El ID del usuario no puede ser cero");
        }
    }
}
