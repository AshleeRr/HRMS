using MyValidator.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMS.Domain.Entities.Reservation;

namespace HRMS.Domain.Base.Validator.ReservationValidator
{
    public class ReservationValidator : Validator<Reservation>
    {
        public ReservationValidator()
        {
            AddRule(r => r != null).WithErrorMessage("La reserva no puede ser nula");
            AddRule(r => r.IdCliente != 0).WithErrorMessage("El ID del cliente no puede ser cero");
            AddRule(r => r.IdHabitacion != 0).WithErrorMessage("El ID de la habitación no puede ser cero");
            AddRule(r => r.FechaEntrada.HasValue).WithErrorMessage("La fecha de entrada no puede ser nula");
            AddRule(r => r.FechaSalida.HasValue).WithErrorMessage("La fecha de salida no puede ser nula");
            AddRule(r => r.FechaEntrada > DateTime.Now).WithErrorMessage("La fecha de entrada debe ser posterior a la fecha actual");
            AddRule(r => r.FechaSalida > r.FechaEntrada).WithErrorMessage("La fecha de salida debe ser posterior a la fecha de entrada");
            AddRule(r => r.Observacion.Length <= 800).WithErrorMessage("La observación es demasiado larga, no puede pasar de 800 caracteres");
        }
    }
}
