using HRMS.Domain.Entities.Users;
using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.UsersValidations
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
                .WithErrorMessage("El número de documento debe de tener entre 6 y 15 caracteres");
            AddRule(c => c.Correo != null && c.Correo.Length <= 50)
                .WithErrorMessage("El correo debe de tener menos de 50 caracteres");
            AddRule(c => c.IdUsuario != null && c.IdUsuario != 1)
                .WithErrorMessage("El id de usuario no puede ser nulo y debe ser 1");
            AddRule(u => ValidateClave(u.Clave))
                .WithErrorMessage("La clave del usuario debe tener al menos 8 caracteres, un número, una letra mayúscula, un caracter especial y una letra minúscula para ser segura");
        }
        private bool ValidateClave(string? clave)
        {
            if (string.IsNullOrEmpty(clave))
                return false;

            if (clave.Length < 12 || clave.Length > 50)
                return false;

            if (!clave.Any(char.IsUpper) || !clave.Any(char.IsDigit) || !clave.Any(char.IsLower))
                return false;

            string caracteresEspeciales = "@#!*?$/,{}=.;:";
            if (!clave.Any(c => caracteresEspeciales.Contains(c)))
                return false;

            return true;
        }
    }
}
