using HRMS.Domain.Entities.Users;
using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.UsersValidations
{
    public class ClientValidator : Validator<Client>
    {
        private readonly PasswordValidations _passwordValidations = new PasswordValidations();
        public ClientValidator()
        {
            AddRule(c => c != null)
                .WithErrorMessage("El cliente no puede ser nulo");
            AddRule(c => c.TipoDocumento != null && c.TipoDocumento.Length <= 15)
                .WithErrorMessage("El tipo de documento debe de tener menos de 15 caracteres");
            AddRule(c => c.Documento != null && c.Documento.Length <= 15 && c.Documento.Length >= 6)
                .WithErrorMessage("El número de documento debe de tener entre 6 y 15 caracteres");
            AddRule(c => c.Correo != null && c.Correo.Length <= 50)
                .WithErrorMessage("El correo debe de tener menos de 50 caracteres");
            //dRule(c => c.IdUsuario >=1)
              //.WithErrorMessage("El id de usuario debe ser especificado");
            AddRule(u => _passwordValidations.ValidateClave(u.Clave))
                .WithErrorMessage("La clave no debe contener espacios. Debe tener entre 12 y 50 caracteres, un número, una letra mayúscula, un caracter especial y una letra minúscula para ser valida");
        }
    }
}
