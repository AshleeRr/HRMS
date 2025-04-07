namespace WebApi.Models.UsersModels.Validations
{
    public class UserValidations
    {
        private readonly DataValidations _validations = new DataValidations();
        OperationResult result = new OperationResult();
        List<string> errors = new List<string>();
        public OperationResult Validate(UserModel model)
        {
            if (model == null)
            {
                errors.Add("El usuario no puede ser nulo.");
            }
            else
            {
                if (string.IsNullOrEmpty(model.NombreCompleto) || model.NombreCompleto.Length > 50)
                    errors.Add("El nombre de usuario debe tener menos de 50 caracteres.");

                if (!_validations.ValidateEmail(model.Correo))
                    errors.Add("El correo debe ser válido y tener menos de 50 caracteres.");

                if (!_validations.ValidateClave(model.Clave))
                    errors.Add("La clave debe de segura.");

                if (string.IsNullOrEmpty(model.TipoDocumento) || model.TipoDocumento.Length > 15)
                    errors.Add("El tipo de documento debe tener menos de 15 caracteres.");

                if (!_validations.ValidateDocumento(model.Documento))
                    errors.Add("El número de documento debe ser válido.");
            }
            result.IsSuccess = errors.Count == 0;
            result.Message = string.Join(Environment.NewLine, errors);

            return result;
        }
    }
}
