using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
//using HRMS.Infraestructure.Notification;

namespace HRMS.Persistence.Repositories.UsersRepository
{
    public class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggingServices _loggerServices;
        private readonly IValidator<User> _validator;
        //private readonly INotificationService _notificationService;
        public UserRepository(HRMSContext context, ILoggingServices loggingServices, //INotificationService notificationService,
                                                   IConfiguration configuration, IValidator<User> validator) : base(context)
        {
            _configuration = configuration;
            _loggerServices = loggingServices;
            _validator = validator;
            //_notificationService = notificationService;
        }

        public async Task<OperationResult> GetUsersByNameAsync(string nombreCompleto)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateNulleable(nombreCompleto, "nombre completo");
                var usuarios = await _context.Users.Where(u => u.NombreCompleto == nombreCompleto).ToListAsync();
                if (!usuarios.Any())
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontraron usuarios con este nombre";
                    await _loggerServices.LogError(result.Message, this, nameof(GetUsersByNameAsync));
                }
                else
                {
                    result.IsSuccess = true;
                    result.Message = "Usuarios encontrados correctamente";
                    result.Data = usuarios;
                }
            } catch(Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }

            return result;
        }
        public async Task<OperationResult> GetUserByEmailAsync(string correo)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateNulleable(correo, "correo");
                var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Correo == correo);
                if (usuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un usuario con este correo";
                    await _loggerServices.LogWarning(result.Message, this, nameof(GetUserByEmailAsync));
                }
                else
                {
                    result.IsSuccess = true;
                    result.Message = "Usuario encontrado correctamente";
                    result.Data = usuario;
                }

            }catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }

            return result;
        }
        public async Task<OperationResult> GetUserByDocumentAsync(string documento)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateNulleable(documento, "documento");
                var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Documento == documento);
                if (usuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un usuario con este documento";
                    await _loggerServices.LogWarning(result.Message, this, nameof(GetUserByDocumentAsync));
                }
                else
                {
                    result.Data = usuario;
                    result.IsSuccess = true;
                    result.Message = "Usuario encontrado correctamente";
                }

            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<OperationResult> GetUsersByTypeDocumentAsync(string tipoDocumento)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateNulleable(tipoDocumento, "tipodocumento");
                var usuarios = await _context.Users.Where(u => u.TipoDocumento == tipoDocumento).ToListAsync();
                if (!usuarios.Any())
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontraron usuarios con este tipo de documentio";
                    await _loggerServices.LogWarning(result.Message, this, nameof(GetUsersByTypeDocumentAsync));
                }
                else
                {
                    result.IsSuccess = true;
                    result.Message = "Usuarios encontrados correctamente";
                    result.Data = usuarios;
                }
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }

            return result;
        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<User, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                var usuarios = await _context.Users.Where(u => u.Estado == true).ToListAsync();
                if (!usuarios.Any())
                {
                    await _loggerServices.LogWarning("No se encontraron usuarios activos", this, nameof(GetAllAsync));
                }
                result.Data = usuarios;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public override async Task<User> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("El id debe ser mayor que 0");
            }
            var entityById = await _context.Users.FindAsync(id);
            if (entityById == null)
            {
                await _loggerServices.LogWarning("No se encontró un usuario con este id", this, nameof(GetEntityByIdAsync));
            }
            return entityById;
        }
        public override async Task<OperationResult> SaveEntityAsync(User entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validUser = _validUser(entity);
                if(!validUser.IsSuccess)
                {
                    return validUser;
                }
                entity.FechaCreacion = DateTime.Now;
                if(entity.IdRolUsuario != 1)
                {
                    int lastReferenceID = await GetLastReferenceIDAsync();
                    entity.ReferenceID = Math.Abs(lastReferenceID + new Random().Next(100, 999));
                }
                else
                {
                    entity.ReferenceID = 1;
                }
                
                result.IsSuccess = true;
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();
                result.Message = "Usuario guardado correctamente";
                //_notificationService.SendNotification(entity.IdUsuario, $"No responda a este mensaje.\nSu usuario ha sido creado con éxito.\nCorreo asociado: {entity.Correo}.\nID de usuario: {entity.ReferenceID}.");
                result.Data = entity;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        private OperationResult _validUser(User user)
        {
            return _validator.Validate(user);
        }
        public override async Task<OperationResult> UpdateEntityAsync(User entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validUser = _validUser(entity);
                if (!validUser.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "Error validando los campos para actualizar";
                    return result;
                }
                var userExistente = await _context.Users.FindAsync(entity.IdUsuario);
                if (userExistente == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Este usuario no existe";
                    return result;
                }
                userExistente.Clave = entity.Clave;
                userExistente.NombreCompleto = entity.NombreCompleto;
                userExistente.Correo = entity.Correo;

                _context.Users.Update(userExistente);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;    
                result.Message = "Usuario actualizado correctamente";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<int> GetLastReferenceIDAsync()
        {
            var lastUserID = await _context.Users.OrderByDescending(u => u.ReferenceID).FirstOrDefaultAsync();
            return lastUserID?.ReferenceID ?? 1000;
        }
        private void ValidateNulleable(string x, string message)
        {
            if (string.IsNullOrEmpty(x))
            {
                _loggerServices.LogError(x, $"El campo: {message} no puede estar vacio.");
                throw new ArgumentException($"El campo: {message} no puede estar vacío.");
            }
        }

        public async Task<bool> UserIDExistsAsync(int userID)
        {
            return await _context.Users.AnyAsync(u => u.ReferenceID == userID);
        }
    }
}
