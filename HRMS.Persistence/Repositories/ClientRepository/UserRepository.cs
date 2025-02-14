using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservation;
using HRMS.Domain.Entities.Users;
using HRMS.Models.Models;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.ClientRepository
{
    public class UserRepository : BaseRepository<Users, int>, IUserRepository
    {
        private readonly HRMSContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(HRMSContext context, ILogger<UserRepository> logger,
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
        public override async Task<bool> ExistsAsync(Expression<Func<Users, bool>> filter)
        {
            if (filter == null)
            {
                return false;
            }
            return await base.ExistsAsync(filter);
        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<Users, bool>> filter)
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
                result.Message = _configuration["ErrorUserRepository: GetAllAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        } 
        public override async Task<Users> GetEntityByIdAsync(int id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("No se encontró un cliente con ese id");
            }
            return entity;
        }
        public override async Task<OperationResult> SaveEntityAsync(Users entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!ValidateUser(entity, result))
                    return result;

                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = "Usuario guardado correctamente.";
                _logger.LogInformation(result.Message);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error al guardar el usuario.";
                _logger.LogError(ex, result.Message);
            }

            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Users entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!ValidateUser(entity, result))
                    return result;

                var usuarioExistente = await _context.Users.FindAsync(entity.IdUsuario);

                if (usuarioExistente == null)
                {
                    result.IsSuccess = false;
                    result.Message = "El usuario no existe en la base de datos.";
                    return result;
                }

                _context.Entry(usuarioExistente).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = "Usuario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error al actualizar el usuario.";
                _logger.LogError(ex, result.Message);
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

        private bool ValidateUser(Users entity, OperationResult result)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "El usuario no puede ser nulo.");
            }
            return true;
        }
    }
}