using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Models.Models;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories
{
    public class UserRepository : BaseRepository<Users, int>, IUserRepository
    {
        private readonly HRMSContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientRepository> _logger;
        public UserRepository(HRMSContext context, ILogger<ClientRepository> logger,
                                                     IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Users> GetUserByName(string nombreCompleto)
        {
            ArgumentException.ThrowIfNullOrEmpty(nombreCompleto, nameof(nombreCompleto));
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.NombreCompleto == nombreCompleto)
                          ?? throw new KeyNotFoundException("Error al encontrar el usuario por nombre");
            return usuario;
        }
        public async Task<OperationResult> GetUsersByUserRolId(int idUsuario)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!ValidateUserId(idUsuario, result))
                    return result;

                var query = await (from users in _context.Users
                                   join userRol in _context.UserRoles on users.IdRolUsuario equals userRol.IdRolUsuario
                                   where users.IdRolUsuario == idUsuario
                                   select new UserModel()
                                   {
                                       IdUsuario = users.IdUsuario,
                                       NombreCompleto = users.NombreCompleto,
                                       IdUserRol = userRol.IdRolUsuario,
                                       UserRol = userRol.Descripcion,
                                       Email = users.Correo
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
                result.Message = _configuration["ErrorUserRepository: GetUserByUserRolId"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());

            }
            return result;
        }

        public async Task<OperationResult> UpdatePassword(int idUsuario, string nuevaClave)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!ValidateUserId(idUsuario, result))
                    return result;

                if (string.IsNullOrEmpty(nuevaClave))
                {
                    result.IsSuccess = false;
                    result.Message = "La nueva clave no puede estar vacía";
                    return result;
                }
                var usuario = await _context.Users.FindAsync(idUsuario);
                if (usuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha podido encontrar el usuario";
                    return result;
                }
                usuario.Clave = nuevaClave;
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Clave actualizada";
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRepository: UpdatePassword"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;

        }
        private bool ValidateUserId(int idUsuario, OperationResult result)
        {
            if (idUsuario < 1)
            {
                result.IsSuccess = false;
                result.Message = "El id del usuario debe ser mayor que 0";
                return false;
            }
            return true;
        }
    }

}
