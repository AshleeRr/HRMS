using HRMS.Domain.Entities.Users;
using MyValidator.Validator;

namespace HRMS.Domain.Base.Validator.UsersValidations
{
    public class UserValidator : Validator<User>
    {
        private readonly DataValidations _validations = new DataValidations();
        public UserValidator()
        {
            AddRule(u => u != null)
                .WithErrorMessage("El usuario no puede ser nulo");
            AddRule(u => u.NombreCompleto != null && u.NombreCompleto.Length <= 50)
                .WithErrorMessage("El nombre de usuario debe tener menos de 50 caracteres");
            AddRule(u => _validations.ValidateEmail(u.Correo))
                .WithErrorMessage("El correo debe de tener menos de 50 caracteres y ser valido");
            AddRule(u => u.IdRolUsuario >= 1)
                .WithErrorMessage("El rol del usuario debe ser mayor que 0");
            AddRule(u => _validations.ValidateClave(u.Clave))
                .WithErrorMessage("La clave no debe contener espacios. Debe tener entre 12 y 50 caracteres, un número, una letra mayúscula, un caracter especial y una letra minúscula para ser valida");
            AddRule(u => u.TipoDocumento != null && u.TipoDocumento.Length <= 15)
                .WithErrorMessage("El tipo de documento debe de tener menos de 15 caracteres");
            AddRule(u => _validations.ValidateDocumento(u.Documento))
                .WithErrorMessage("El número de documento debe de tener entre 6 y 15 caracteres y ser valido");
        }
    }
}
