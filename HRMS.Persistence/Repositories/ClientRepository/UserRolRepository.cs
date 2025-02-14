using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.ClientRepository
{
    public class UserRolRepository : BaseRepository<UserRole, int>, IUserRoleRepository
    {
        private readonly HRMSContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRolRepository> _logger;


        public UserRolRepository(HRMSContext context, ILogger<UserRolRepository> logger,
                                                     IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<UserRole> GetRoleByDescription(string descripcion)
        {
            return await _context.UserRoles.AsNoTracking().FirstOrDefaultAsync(ur => ur.Descripcion == descripcion);
         
        }
        public async Task<OperationResult> UpdateDescription(int idRolUsuario, string nuevaDescripcion)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (idRolUsuario <= 0)
                {
                    result.IsSuccess = false;
                    result.Message = "El id del rol de usuario debe ser mayor que 0";
                    return result;
                }
                if (string.IsNullOrEmpty(nuevaDescripcion))
                {
                    result.IsSuccess = false;
                    result.Message = "La descripción del rol del usuario no puede estar vacía";
                    return result;
                }
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
                result.Message = _configuration["ErrorUserRolRepository: UpdateDescription"];
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
                if (filter != null)
                {
                    return await base.GetAllAsync(filter);
                }
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

        public override async Task<OperationResult> SaveEntityAsync(UserRole entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "El rol de usuario no puede ser nulo.");
                }

                await _context.UserRoles.AddAsync(entity);
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
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "El rol no puede ser nulo. Intente otra vez");
                }

                var rolUsuarioExistente = await _context.UserRoles.FindAsync(entity.IdRolUsuario);

                if (rolUsuarioExistente == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Este rol no existe";
                    return result;
                }

                _context.Entry(rolUsuarioExistente).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = "Rol de usuario actualizado correctamente.";
                _logger.LogInformation(result.Message);
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRolRepository: UpdateEntityAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }

            return result;
        }
    }
}
