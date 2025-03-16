using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Models.Models.UsersModels;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.UsersRepository
{
    public class UserRoleRepository : BaseRepository<UserRole, int>, IUserRoleRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggingServices _loggerServices;
        private readonly IValidator<UserRole> _validator;
        public UserRoleRepository(HRMSContext context, ILoggingServices loggingServices,
                                                     IConfiguration configuration, IValidator<UserRole> validator) : base(context)
        {
            _configuration = configuration;
            _loggerServices = loggingServices;
            _validator = validator;
        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<UserRole, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                var roles = await _context.UserRoles.Where(ur => ur.Estado == true).ToListAsync();
                if (!roles.Any())
                {
                   await _loggerServices.LogWarning("No se encontraron roles activos", this, nameof(GetAllAsync));
                }
                result.Data = roles;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public override async Task<UserRole> GetEntityByIdAsync(int id)
        {
            ValidateId(id);
            var entity = await _context.UserRoles.FindAsync(id);
            if (entity == null)
            {
                await _loggerServices.LogWarning("No se encontró un rol con este id", this, nameof(GetEntityByIdAsync));
            }
            return entity;
        }
        public override async Task<OperationResult> SaveEntityAsync(UserRole entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validUserRole = _validUserRole(entity);
                if (!validUserRole.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "Error validando los campos del rol para guardar";
                    return result;
                }
                entity.FechaCreacion = DateTime.Now;
                result.IsSuccess = true;
                
                await _context.UserRoles.AddAsync(entity);
                await _context.SaveChangesAsync();
                result.Message = "Rol de usuario guardado correctamente.";
                result.Data = entity;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        private OperationResult _validUserRole(UserRole userRole)
        {
            return _validator.Validate(userRole);
        }
        public override async Task<OperationResult> UpdateEntityAsync(UserRole entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validUserRole = _validUserRole(entity);
                if(!validUserRole.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "Error validando los campos del rol para actualizar";
                    return result;
                }
                var rolUsuario = await _context.UserRoles.FindAsync(entity.IdRolUsuario);
                if (rolUsuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Este rol no existe";
                    return result;
                }

                rolUsuario.Descripcion = entity.Descripcion;
                rolUsuario.RolNombre = entity.RolNombre;

                _context.UserRoles.Update(rolUsuario);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Rol de usuario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public async Task<UserRole> GetRoleByNameAsync(string rolNombre)
        {
            ValidateNulleable(rolNombre, "rol nombre");
            var rolUsuario = await _context.UserRoles.AsNoTracking().FirstOrDefaultAsync(ur => ur.RolNombre == rolNombre);
            if (rolUsuario == null)
            {
                await _loggerServices.LogError("No se encontró un rol con este nombre", this, nameof(GetRoleByNameAsync));
            }
            return rolUsuario;
        }
        public async Task<UserRole> GetRoleByDescriptionAsync(string descripcion)
        {
            ValidateNulleable(descripcion, "descripcion");
            var rolUsuario = await _context.UserRoles.AsNoTracking().FirstOrDefaultAsync(ur => ur.Descripcion == descripcion);
            if(rolUsuario == null)
            {
                await _loggerServices.LogWarning("No se encontró un rol con esta descripción", this, nameof(GetRoleByDescriptionAsync));
            }
            return rolUsuario;
        }
        public async Task<OperationResult> GetUsersByUserRoleIdAsync(int id)
        {
            OperationResult result = new OperationResult();
            try
            {
                ValidateId(id);
                var query = await (from userRol in _context.UserRoles
                                   join users in _context.Users on userRol.IdRolUsuario equals users.IdRolUsuario
                                   where userRol.IdRolUsuario == id
                                   select new UserModel()
                                   {
                                       IdUsuario = users.IdUsuario,
                                       NombreCompleto = users.NombreCompleto,
                                       IdUserRol = userRol.IdRolUsuario,
                                       Correo = users.Correo,
                                       UserRol = userRol.Descripcion,
                                       Email = users.Correo,
                                       TipoDocumento = users.TipoDocumento,
                                       Documento = users.Documento,
                                   }).ToListAsync();
                result.Data = query;
                result.IsSuccess = true;
                if (!query.Any())
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontraron usuarios con este rol";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        private void ValidateId(int id)
        {
            if (id <= 0)
            {
                _loggerServices.LogError($"{id}", "El id debe ser mayor que 0");
            }
        }
        private void ValidateNulleable(string x, string message)
        {
            if (string.IsNullOrEmpty(x))
            {
                _loggerServices.LogError(x, $"El campo: {message} no puede estar vacio.");
            }
        }
    }
}
