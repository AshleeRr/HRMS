namespace WebApi.Models.UsersModels.UsersValidations
{
    public class DataValidations
    {
        public bool ValidateClave(string? clave)
        {
            if (string.IsNullOrEmpty(clave))
                return false;

            if (clave.Length < 12 || clave.Length > 50)
                return false;

            if (!clave.Any(char.IsUpper) || !clave.Any(char.IsDigit) || !clave.Any(char.IsLower))
                return false;

            string caracteresEspeciales = "@#!*?$/,{}=.;:_-";
            if (!clave.Any(c => caracteresEspeciales.Contains(c)))
                return false;

            if (clave.Contains(" "))
                return false;

            return true;
        }
        public bool ValidateEmail(string? email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            if (email.Length < 5 || email.Length > 50)
                return false;

            if (email.Contains(" "))
                return false;

            string patron = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, patron);
        }
        public bool ValidateDocumento(string? documento)
        {
            if (string.IsNullOrEmpty(documento))
                return false;

            if (documento.Length < 6 || documento.Length > 15)
                return false;

            if (documento.Contains(" "))
                return false;

            string patron = @"^[a-zA-Z0-9]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(documento, patron);
        }
    }
}
