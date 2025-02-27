
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
    public class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(HRMSContext context, ILogger<UserRepository> logger,
                                                     IConfiguration configuration) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<User> AuthenticateUserAsync(string correo, string clave)
        {

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(clave))
            {
                throw new ArgumentNullException("El correo y la clave no pueden estar vacíos");
            }
            var AuthenticatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Correo == correo && u.Clave == clave);
            if (AuthenticatedUser == null)
            {
                _logger.LogWarning("No se encontró un usuario con ese correo y clave");
            }
            return AuthenticatedUser;
        }
        public async Task<OperationResult> UpdateEstadoAsync(User entity, bool nuevoEstado)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!Validation.ValidateUser(entity, result))
                    return result;
                if (entity.Estado == nuevoEstado)
                {
                    result.IsSuccess = false;
                    _logger.LogWarning("No se puede asignar el mismo estado al usuario");
                    return result;
                }
                var userExistente = await _context.Users.AnyAsync(u => u.IdUsuario == entity.IdUsuario);
                if (!userExistente)
                {
                    result.IsSuccess = false;
                    result.Message = "Este usuario no existe";
                    return result;

                }
                else
                {
                    var user = await _context.Users.FindAsync(entity.IdUsuario);
                    user.Estado = nuevoEstado;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Message = "Estado del usuario actualizado correctamente";
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = _configuration["ErrorUserRepository: UpdateEstadoAsync"];
                _logger.LogError(result.Message, e.ToString());
            }
            return result;
        }

        public async Task<List<User>> GetUsersByNameAsync(string nombreCompleto)
        {
            ArgumentException.ThrowIfNullOrEmpty(nombreCompleto, nameof(nombreCompleto));
            var usuarios = await _context.Users.Where(u => u.NombreCompleto == nombreCompleto).ToListAsync();
            if (!usuarios.Any())
            {
                _logger.LogWarning("No se encontraron usuarios activos");
            }
            return usuarios;
        }
        public async Task<OperationResult> GetUsersByUserRoleIdAsync(int id)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!Validation.ValidateId(id, result))
                    return result;

                var query = await (from users in _context.Users
                                   join userRol in _context.UserRoles on users.IdRolUsuario equals userRol.IdRolUsuario
                                   where users.IdRolUsuario == id
                                   select new UserModel()
                                   {
                                       IdUsuario = users.IdUsuario,
                                       NombreCompleto = users.NombreCompleto,
                                       IdUserRol = userRol.IdRolUsuario,
                                       Correo = users.Correo,
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
        public async Task<OperationResult> UpdatePasswordAsync(int idUsuario, string nuevaClave)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!Validation.ValidateId(idUsuario, result))
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
        //metodos de baserepository
        public override async Task<bool> ExistsAsync(Expression<Func<User, bool>> filter)
        {
            if (filter == null)
            {
                return false;
            }
            return await base.ExistsAsync(filter);
        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<User, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                var usuarios = await _context.Users.Where(u => u.Estado == true).ToListAsync();
                if (!usuarios.Any())
                {
                    _logger.LogWarning("No se encontraron usuarios activos");
                }
                result.Data = usuarios;
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
        public override async Task<User> GetEntityByIdAsync(int id)
        {
            var entityById = await _context.Users.FindAsync(id);
            if (entityById == null)
            {
                _logger.LogWarning("No se encontró un usuario con ese id");
            }
            return entityById;
        }
        public override async Task<OperationResult> SaveEntityAsync(User entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!Validation.ValidateUser(entity, result))
                    return result;
                if (!Validation.ValidateCompleteName(entity.NombreCompleto, entity.IdUsuario, result))
                    return result;
                if (!await Validation.ValidateCorreo(entity.Correo, entity.IdUsuario, _context, result))
                    return result;
                if (!Validation.ValidateId(entity.IdRolUsuario, result))
                    return result;
                if (!Validation.ValidateClave(entity.Clave, result))
                    return result;

                entity.FechaCreacion = DateTime.Now;
                result.IsSuccess = true;
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();

                result.Message = "Usuario guardado correctamente";
                _logger.LogInformation(result.Message);
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error al guardar el usuario";
                _logger.LogError(ex, result.Message);
            }
            return result;
        }
        public override async Task<OperationResult> UpdateEntityAsync(User entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!Validation.ValidateUser(entity, result))
                    return result;
                if (!Validation.ValidateId(entity.IdUsuario, result))
                    return result;
                if (!Validation.ValidateCompleteName(entity.NombreCompleto, entity.IdUsuario, result))
                    return result;
                if (!await Validation.ValidateCorreo(entity.Correo, entity.IdUsuario, _context, result))
                    return result;
                if (!Validation.ValidateClave(entity.Clave, result))
                    return result;
                var userExistente = await _context.Users.AnyAsync(u => u.IdRolUsuario == entity.IdRolUsuario);
                if (!userExistente)
                {
                    result.IsSuccess = false;
                    result.Message = "Este usuario no existe";
                }
                else
                {
                    var usuario = await _context.Users.FindAsync(entity.IdUsuario);
                    usuario.Estado = entity.Estado;
                    usuario.Clave = entity.Clave;
                    usuario.NombreCompleto = entity.NombreCompleto;
                    usuario.Correo = entity.Correo;
                    _context.Users.Update(usuario);
                    await _context.SaveChangesAsync();
                    result.Message = "Usuario actualizado correctamente";

                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error al actualizar el usuario";
                _logger.LogError(ex, result.Message);
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }
    }
}
