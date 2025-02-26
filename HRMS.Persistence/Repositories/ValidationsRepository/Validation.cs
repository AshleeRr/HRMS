using HRMS.Domain.Base;
using HRMS.Domain.Entities.Audit;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.ValidationsRepository
{
    public static class Validation
    {
        // metodos de validaciones
        public static bool ValidateClave(string clave, OperationResult result)
        {
            if (string.IsNullOrEmpty(clave))
            {
                result.IsSuccess = false;
                result.Message = "La clave no puede estar vacía";
                return false;
            }
            if (clave.Length < 8)
            {
                result.IsSuccess = false;
                result.Message = "La clave es muy corta, no es segura";
                return false;
            }
            if (clave.Length > 50)
            {
                result.IsSuccess = false;
                result.Message = "La clave es muy larga";
                return false;
            }
            if (!clave.Any(char.IsDigit))
            {
                result.IsSuccess = false;
                result.Message = "La clave debe contener al menos un número para ser segura";
                return false;
            }
            if (!clave.Any(char.IsUpper))
            {
                result.IsSuccess = false;
                result.Message = "La clave debe contener al menos una letra mayúscula para ser segura";
                return false;
            }
            if (!clave.Any(char.IsLower))
            {
                result.IsSuccess = false;
                result.Message = "La clave debe contener al menos una letra minúscula para ser segura";
                return false;
            }
            result.IsSuccess = true;
            return true;
        }
        public static bool ValidateAction(string Accion, OperationResult result)
        {
            if (Accion == null || Accion.Length > 75)
            {
                result.IsSuccess = false;
                result.Message = "La acción no puede ser nula o tener más de 50 caracteres";
                return false;
            }
            if (!Accion.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                result.Message = "La acción realizada solo puede contener letras y espacios.";
                result.IsSuccess = false;
                return false;
            }

            return true;
        }
        public static bool ValidateCompleteName(string NombreCompleto, int idCliente, OperationResult result)
        {
            if (NombreCompleto == null || NombreCompleto.Length > 50)
            {
                result.IsSuccess = false;
                result.Message = "El nombre no puede ser nulo o tener más de 50 caracteres";
                return false;
            }
            if (!NombreCompleto.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                result.Message = "El nombre solo puede contener letras y espacios.";
                result.IsSuccess = false;
                return false;
            }
            return true;
        }
        public static bool ValidateDescription(string Descripcion, OperationResult result)
        {
            if (string.IsNullOrEmpty(Descripcion))
            {
                result.IsSuccess = false;
                result.Message = "La descripción no puede estar vacía";
                return false;
            }
            if (Descripcion.Length > 50)
            {
                result.Message = "La descripción del rol de usuario no puede tener más de 50 caracteres";
                result.IsSuccess = false;
                return false;
            }
            if (Descripcion.Length < 5)
            {
                result.Message = "La descripción es muy corta, sea más desciptivo por seguridad";
                result.IsSuccess = false;
                return false;
            }
            if (!Descripcion.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                result.Message = "La descripción solo puede contener letras y espacios";
                result.IsSuccess = false;
                return false;
            }
            return true;
        }
        public static async Task<bool> ValidateCorreo(string correo, int idCliente, HRMSContext context, OperationResult result)
        {

            if(string.IsNullOrEmpty(correo) || correo.Length > 50)

            {
                result.IsSuccess = false;
                result.Message = "El correo no puede ser nulo o tener más de 50 caracteres";
                return false;
            }
            bool exists = await context.Clients.AnyAsync(c => c.Correo == correo);
            if (exists)
            {
                result.IsSuccess = false;
                result.Message = "Este correo ya está registrado";
                return false;
            }
            return true;
        }

        
       public static bool ValidateTipoDocumento(string TipoDocumento, int idCliente, OperationResult result)

        {
            if (TipoDocumento == null || TipoDocumento.Length > 15)
            {
                result.IsSuccess = false;
                result.Message = "El tipo de documento no puede ser nulo o tener más de 15 caracteres";
                return false;
            }
            if (!TipoDocumento.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                result.Message = "El tipo de documetno solo puede contener letras y espacios.";
                result.IsSuccess = false;
                return false;
            }
            return true;
        }
        public static async Task<bool> ValidateDocumento(string documento, int idCliente, HRMSContext context, OperationResult result)
        {
            if (string.IsNullOrEmpty(documento) || documento.Length > 15)
            {
                result.IsSuccess = false;
                result.Message = "El documento no puede ser nulo o tener más de 15 caracteres";
                return false;
            }
            bool exists = await context.Clients.AnyAsync(c => c.Documento == documento);
            if (exists)
            {
                result.IsSuccess = false;
                result.Message = "Este documento de identidad ya está registrado";
                return false;
            }
            return true;
        }
        public static bool ValidateId(int id, OperationResult result)
        {
            if (id <= 0)
            {
                result.IsSuccess = false;
                result.Message = "El id debe ser mayor que 0";
                return false;
            }
            return true;
        }
        public static bool ValidateUser(User entity, OperationResult result)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "El usuario no puede ser nulo");
            }
            return true;
        }
        public static bool ValidateClient(Client entity, OperationResult result)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "El cliente no puede ser nulo");
            }
            return true;
        }
        public static bool ValidateUserRole(UserRole entity, OperationResult result)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "El rol del usuario no puede ser nulo");
            }
            return true;
        }
    }
}