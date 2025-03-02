using HRMS.Domain.Entities.Users;
using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.ClientValidations
{
    public class ClientValidator : Validator<Client>
    {
        public ClientValidator()
        {
            AddRule(c => c != null)
                .WithErrorMessage("El cliente no puede ser nulo");
            AddRule(c => c.TipoDocumento != null && c.TipoDocumento.Length <= 15)
                .WithErrorMessage("El tipo de documento debe de tener menos de 15 caracteres");
            AddRule(c => c.Documento != null && c.Documento.Length <= 15 && c.Documento.Length >= 6)
                .WithErrorMessage("El número de documento debe de tener ENTRE 6 y 15 caracteres");
            AddRule(c => c.Correo != null && c.Correo.Length <= 50)
                .WithErrorMessage("El correo debe de tener menos de 50 caracteres");
        }
    }
}
