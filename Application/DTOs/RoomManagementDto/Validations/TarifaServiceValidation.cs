using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using MyValidator.Validator;
using System;

namespace HRMS.Application.DTOs.RoomManagementDto.Validations
{
    public class TarifaServiceValidation : Validator<CreateTarifaDto>
    {
        public TarifaServiceValidation()
        {
       
            AddRule(r => !string.IsNullOrEmpty(r.Descripcion) && r.Descripcion.Length > 255)
                .WithErrorMessage("La descripción no puede tener más de 255 caracteres");
            
            AddRule(r => !string.IsNullOrEmpty(r.Descripcion) && r.Descripcion.Length < 3)
                .WithErrorMessage("La descripción debe tener al menos 3 caracteres");
                
            AddRule(r => string.IsNullOrEmpty(r.Descripcion))
                .WithErrorMessage("La descripción no puede estar vacía");
            
            AddRule(r => r.FechaInicio >= r.FechaFin)
                .WithErrorMessage("La fecha de inicio debe ser menor a la fecha de fin");
            
            AddRule(r => r.FechaInicio <= DateTime.Now)
                .WithErrorMessage("La fecha de inicio tiene que ser mayor a la fecha actual");
            AddRule(r => r.PrecioPorNoche <= 0)
                .WithErrorMessage("El precio por noche debe ser mayor que 0");
            
            AddRule(r => r.Descuento < 0)
                .WithErrorMessage("El descuento no puede ser negativo");
            
            AddRule(r => r.IdCategoria <= 0)
                .WithErrorMessage("El id de categoría debe ser mayor que 0");
        }
    }
}