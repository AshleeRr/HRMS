using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.ReservationValidator
{
    public class ReservationValidator : Validator<Entities.Reservations.Reservation>
    {
        public ReservationValidator()
        {
            AddRule(r => r != null)
                .WithErrorMessage("La reserva no puede ser nula");
            AddRule(r => r.IdCliente != 0)
                .WithErrorMessage("El ID del cliente no puede ser cero");
            AddRule(r => r.IdHabitacion != 0)
                .WithErrorMessage("El ID de la habitación no puede ser cero");
            AddRule(r => r.FechaEntrada.HasValue)
                .WithErrorMessage("La fecha de entrada no puede ser nula");
            AddRule(r => r.FechaSalida.HasValue)
                .WithErrorMessage("La fecha de salida no puede ser nula");
            AddRule(r => r.FechaEntrada > DateTime.Now)
                .WithErrorMessage("La fecha de entrada debe ser posterior a la fecha actual");
            AddRule(r => r.FechaSalida > r.FechaEntrada)
                .WithErrorMessage("La fecha de salida debe ser posterior a la fecha de entrada");
            AddRule(r => r.Observacion.Length <= 800)
                .WithErrorMessage("La observación es demasiado larga, no puede pasar de 800 caracteres");
            AddRule(r => isMoreThanCero(r.PrecioInicial) && isValidForDecimal10_2(r.PrecioInicial))
                .WithErrorMessage("El precio inicial no es una cantidad valida");
            AddRule(r => r.PrecioRestante >= 0 && isValidForDecimal10_2(r.PrecioRestante))
                .WithErrorMessage("El precio restante no  es una cantidad valida");
            AddRule(r => isMoreThanCero(r.TotalPagado) && isValidForDecimal10_2(r.TotalPagado))
                .WithErrorMessage("El Total pagado no es una cantidad valida");
            AddRule(r => r.CostoPenalidad >=  0 && isValidForDecimal10_2(r.CostoPenalidad))
                .WithErrorMessage("El costo penalidad no es una cantidad valida");

        }


        private bool isValidForDecimal10_2(decimal? num)
        {
            if (!num.HasValue)
                return false;
            var numero = num.Value;
            
            decimal parteEntera = Math.Truncate(numero);
            decimal parteDecimal = numero - parteEntera;

            int digitosEnteros = parteEntera == 0 ? 1 : (int)Math.Floor(Math.Log10((double)Math.Abs(parteEntera)) + 1);

            return digitosEnteros <= 8 && Math.Round(parteDecimal, 2) == parteDecimal;
        }

        private bool isMoreThanCero(decimal? num)
        {
            if (!num.HasValue)
                return false;
            var numero = num.Value;
            return num > 0;
        }
    }
}
