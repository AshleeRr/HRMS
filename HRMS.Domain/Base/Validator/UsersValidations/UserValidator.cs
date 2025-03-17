using HRMS.Domain.Entities.Users;
using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.UsersValidations
{
    public class UserValidator : Validator<User>
    {
        public UserValidator()
        {
            AddRule(u => u != null)
                .WithErrorMessage("El usuario no puede ser nulo");
            AddRule(u => u.NombreCompleto != null && u.NombreCompleto.Length <= 50)
                .WithErrorMessage("El nombre de usuario debe tener menos de 50 caracteres");
            AddRule(u => u.Correo != null && u.Correo.Length <= 50)
                .WithErrorMessage("El apellido de usuario debe tener menos de 50 caracteres");
            AddRule(u => u.IdRolUsuario >= 1)
                .WithErrorMessage("El rol del usuario debe ser mayor que 0");
            AddRule(u => ValidateClave(u.Clave))
                .WithErrorMessage("La clave no debe contener espacios. Debe tener al menos 8 caracteres, un número, una letra mayúscula, un caracter especial y una letra minúscula para ser segura");
            AddRule(u => u.TipoDocumento != null && u.TipoDocumento.Length <= 15)
                .WithErrorMessage("El tipo de documento debe de tener menos de 15 caracteres");
            AddRule(u => u.Documento != null && u.Documento.Length <= 15 && u.Documento.Length >= 6)
                .WithErrorMessage("El número de documento debe de tener ENTRE 6 y 15 caracteres");
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

            if (clave.Contains(" "))
                return false;

            return true;
        }
    }
}
