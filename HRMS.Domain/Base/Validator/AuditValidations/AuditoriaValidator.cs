using HRMS.Domain.Entities.Audit;
using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.AuditValidations
{
    public class AuditoriaValidator : Validator<Auditoria>
    {
        public AuditoriaValidator() 
        {
            AddRule(a => a != null)
                .WithErrorMessage("La auditoria no puede ser nula");
            AddRule(a => a.Accion.Length <= 75)
                .WithErrorMessage("La accion no debe tener mas de 75 caracteres");
            AddRule(a => a.IdUsuario >= 1)
                .WithErrorMessage("El id del usuario de creación debe ser mayor que 0 ");
        }
    }
}
