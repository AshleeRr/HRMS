using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using MyValidator.Validator;

namespace HRMS.Application.DTOs.RoomManagementDto.Validations;

public class TarifaServiceValidation : Validator<CreateTarifaDto>
{
    public TarifaServiceValidation()
    {
        AddRule(r => r.PrecioPorNoche <= 0).WithErrorMessage("El precio por noche no puede ser 0 o negativo");
        AddRule(r => r.IdCategoria <= 0).WithErrorMessage("El id de categoria no puede ser 0 o negativo"); 
        AddRule(r => r.FechaInicio > DateTime.Now).WithErrorMessage("La fecha de inicio tiene que ser mayor a la fecha actual");
        AddRule(r => r.Descuento < 0).WithErrorMessage("El descuento no puede ser negativo");
        AddRule(r => r.Descripcion.Length <= 255).WithErrorMessage("La descripción no puede tener más de 255 caracteres");
        AddRule(r => r.Descripcion.Length >= 3).WithErrorMessage("La descripción debe tener al menos 3 caracteres");
        AddRule(t=> t.FechaInicio < t.FechaFin).WithErrorMessage("La fecha de inicio debe ser menor a la fecha de fin");
    }
}