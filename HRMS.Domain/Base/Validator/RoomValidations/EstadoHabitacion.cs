namespace HRMS.Domain.Base.Validator.RoomValidations;

public class EstadoHabitacion : Validator<Entities.RoomManagement.EstadoHabitacion>
{
    public EstadoHabitacion()
    {
        AddRule(e=> e != null).WithErrorMessage("El estado de la habitación no puede ser nulo");
        AddRule(e => e.Descripcion != null && e.Descripcion.Length<= 50 ).WithErrorMessage(
            "La descripción no puede ser nula y debe tener menos de 50 caracteres");
        
        
    }
}