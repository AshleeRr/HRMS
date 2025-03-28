
namespace HRMS.Application.DTOs.UsersDTOs.ValidationsForSaveDTOs
{
    public class PasswordValidations
    {
        public bool ValidateClave(string? clave)
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
