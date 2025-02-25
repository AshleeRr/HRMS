using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Models.Models.UsersModels;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Persistence.Repositories.ValidationsRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.ClientRepository
{
    public class UserRoleRepository : BaseRepository<UserRole, int>, IUserRoleRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRoleRepository> _logger;
        public UserRoleRepository(HRMSContext context, ILogger<UserRoleRepository> logger,
                                                     IConfiguration configuration) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }
        
        public async Task<UserRole> GetRoleByDescriptionAsync (string descripcion)
        {
            return await _context.UserRoles.AsNoTracking().FirstOrDefaultAsync(ur => ur.Descripcion == descripcion);
         
        }
        public async Task<OperationResult> UpdateDescriptionAsync(int idRolUsuario, string nuevaDescripcion)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!Validation.ValidateId(idRolUsuario, result))
                    return result;

                if(!Validation.ValidateDescription(nuevaDescripcion, result))
                    return result;
                var rolUsuario = await _context.UserRoles.FindAsync(idRolUsuario);
                if (rolUsuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un rol de usuario con ese id";
                    return result;
                }
                rolUsuario.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Se actualizó la descripción del rol de usuario";
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRolRepository: UpdateDescriptionAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }
        public override async Task<bool> ExistsAsync(Expression<Func<UserRole, bool>> filter)
        {
            if (filter == null)
            {
                return false;
            }
            return await base.ExistsAsync(filter);
        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<UserRole, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                var roles = await _context.UserRoles.Where(ur => ur.Estado == true).ToListAsync();
                if (!roles.Any())
                {
                    _logger.LogWarning("No se encontraron usuarios activos");
                }
                result.Data = roles;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRolRepository: GetAllAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public override async Task<UserRole> GetEntityByIdAsync(int id)
        {
            var entity = await _context.UserRoles.FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("No se encontró un cliente con ese id");
            }
            return entity;
        }
        public async Task<OperationResult> AsignRolUserAsync(int idUsuario, int idRolUsuario)
        { // asigna un rol a un usuario
            OperationResult result = new OperationResult();
            try
            {
                if (!Validation.ValidateId(idUsuario, result))
                    return result;
                if (!Validation.ValidateId(idRolUsuario, result))
                    return result;
                var usuario = await _context.Users.FindAsync(idUsuario);
                if (usuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un usuario con ese id";
                    return result;
                }
                var userRol = await _context.UserRoles.FindAsync(idRolUsuario);
                if (userRol == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se encontró un rol de usuario con ese id";
                    return result;
                }
                usuario.IdRolUsuario = idRolUsuario;
                _context.Users.Update(usuario);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Rol asignado correctamente";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = _configuration["ErrorUserRepository: AsignRolUserAsync"];
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }
        public override async Task<OperationResult> SaveEntityAsync(UserRole entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                if(!Validation.ValidateUserRole(entity, result))
                    return result;
                
                if(!Validation.ValidateDescription(entity.Descripcion, result))
                    return result;
                _context.UserRoles.Update(entity);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Rol de usuario guardado correctamente.";
                _logger.LogInformation(result.Message);
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRolRepository: SaveEntityAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }

            return result;
        }
        public override async Task<OperationResult> UpdateEntityAsync(UserRole entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                if(!Validation.ValidateUserRole(entity, result))
                    return result;
                if(!Validation.ValidateId(entity.IdRolUsuario, result))
                    return result;
                if(!Validation.ValidateDescription(entity.Descripcion, result))
                    return result;
                var rolUsuarioExistente = await _context.UserRoles.FindAsync(entity.IdRolUsuario);
                if (rolUsuarioExistente == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Este rol no existe";
                    return result;
                }
                rolUsuarioExistente.Descripcion = entity.Descripcion;
                rolUsuarioExistente.Estado = entity.Estado;
                _context.UserRoles.Update(rolUsuarioExistente);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRolRepository: UpdateEntityAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }
    }
}
