using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.ClientRepository
{
    public class UserRolRepository : BaseRepository<UserRole, int>, IUserRoleRepository
    {
        private readonly HRMSContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Repositories.ClientRepository.ClientRepository> _logger;
        public UserRolRepository(HRMSContext context, ILogger<Repositories.ClientRepository.ClientRepository> logger,
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
    }
}
