using HRMS.Application.DTOs.UsersDTOs.UserDTOs;
using MyValidator.Validator;

namespace HRMS.Application.DTOs.UsersDTOs.ValidationsForSaveDTOs
{
    public class UserServiceValidator : Validator<SaveUserDTO>
    {
        private readonly DataValidations _validations = new DataValidations();

        public UserServiceValidator()
        {
            AddRule(u => u != null)
                .WithErrorMessage("El usuario no puede ser nulo");
            AddRule(u => u.NombreCompleto != null && u.NombreCompleto.Length <= 50)
                .WithErrorMessage("El nombre de usuario debe tener menos de 50 caracteres");
            AddRule(u => _validations.ValidateEmail(u.Correo))
                .WithErrorMessage("El correo debe de tener menos de 50 caracteres y ser valido");
            AddRule(u => _validations.ValidateClave(u.Clave))
                .WithErrorMessage("La clave no debe contener espacios. Debe tener entre 12 y 50 caracteres, un número, una letra mayúscula, un caracter especial y una letra minúscula para ser valida");
            AddRule(u => u.TipoDocumento != null && u.TipoDocumento.Length <= 15)
                .WithErrorMessage("El tipo de documento debe de tener menos de 15 caracteres");
            AddRule(u => _validations.ValidateDocumento(u.Documento))
                .WithErrorMessage("El número de documento debe de tener entre 6 y 15 caracteres y ser valido");
        }
    }
}
