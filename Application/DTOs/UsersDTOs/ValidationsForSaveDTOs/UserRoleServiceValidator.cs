using HRMS.Application.DTOs.UsersDTOs.UserRoleDTOs;
using MyValidator.Validator;

namespace HRMS.Application.DTOs.UsersDTOs.ValidationsForSaveDTOs
{
    public class UserRoleServiceValidator : Validator<SaveUserRoleDTO>
    {
        public UserRoleServiceValidator()
        {
            AddRule(ur => ur != null)
                .WithErrorMessage("El rol de usuario no puede ser nulo");
            AddRule(ur => ur.Descripcion != null && ur.Descripcion.Length <= 50)
                .WithErrorMessage("La descripcion rol de usuario debe tener menos de 50 caracteres");
            AddRule(ur => ur.Nombre != null && ur.Nombre.Length <= 30)
                .WithErrorMessage("El nombre del rol de usuario debe tener menos de 50 caracteres");
            AddRule(ur => ur.UserID >= 1)
                .WithErrorMessage("El id del usuario debe ser mayor que 0 para la auditoria");
        }

    }
}
