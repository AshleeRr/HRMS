namespace WebApi.Models.UsersModels.Validations
{
    public class UserRoleValidations
    {
        OperationResult result = new OperationResult();
        List<string> errors = new List<string>();
        public OperationResult Validate(UserRoleModel model)
        {
            if (model == null)
            {
                errors.Add("El rol de usuario no puede ser nulo.");
            }
            else
            {
                if (string.IsNullOrEmpty(model.RolNombre) || model.RolNombre.Length > 30)
                    errors.Add("El nombre del rol de usuario debe tener menos de 30 caracteres.");

                if (string.IsNullOrEmpty(model.Descripcion) || model.Descripcion.Length > 50)
                    errors.Add("La descripción del rol de usuario debe tener menos de 50 caracteres.");
            }

            result.IsSuccess = errors.Count == 0;
            result.Message = string.Join(Environment.NewLine, errors);// une los errores con saltitos de linea

            return result;
        }
    }
}

