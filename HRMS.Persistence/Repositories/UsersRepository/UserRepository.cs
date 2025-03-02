using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Models.Models.UsersModels;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.UsersRepository
{
    public class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRepository> _logger;
        private readonly IValidator<User> _validator;

        public UserRepository(HRMSContext context, ILogger<UserRepository> logger,
                                                     IConfiguration configuration, IValidator<User> validator) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
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
        // nuevo, aregar a api
        public async Task<User> GetUserByEmailAsync(string correo)
        {
            ArgumentException.ThrowIfNullOrEmpty(correo, nameof(correo));
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
            {
                _logger.LogWarning("No se encontró un usuario con ese correo");
            }
            return usuario;
        }
        public async Task<OperationResult> GetUsersByUserRoleIdAsync(int id)
        {
            OperationResult result = new OperationResult();
            try
            {
                if(id <= 0)
                {
                    result.IsSuccess = false;
                    result.Message = "El id del rol de usuario debe ser mayor que 0";
                    return result;
                }
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
            OperationResult resultSave = new OperationResult();
            try
            {
                var validUser = _validator.Validate(entity);
                if(!validUser.IsSuccess)
                {
                    return validUser;
                }
                entity.FechaCreacion = DateTime.Now;
                resultSave.IsSuccess = true;
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();

                resultSave.Message = "Usuario guardado correctamente";
                return resultSave;
            }
            catch (Exception ex)
            {
                resultSave.IsSuccess = false;
                resultSave.Message = "Ocurrió un error al guardar el usuario";
                _logger.LogError(ex, resultSave.Message);
            }
            return resultSave;
        }
        private OperationResult _validUserForUpdateMethod(User user)
        {
            return _validator.Validate(user);
        }
        public override async Task<OperationResult> UpdateEntityAsync(User entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validUser = _validator.Validate(entity);
                if (!validUser.IsSuccess)
                {
                    return validUser;
                }
                var userExistente = await _context.Users.FindAsync(entity.IdUsuario);
                if (userExistente == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Este usuario no existe";
                    return result;
                }
                var usuario = await _context.Users.FindAsync(entity.IdUsuario);
                usuario.Clave = entity.Clave;
                usuario.NombreCompleto = entity.NombreCompleto;
                usuario.Correo = entity.Correo;

                _context.Users.Update(usuario);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;    
                result.Message = "Usuario actualizado correctamente";
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



        // metodos que no van
        /*
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

        }*/

        /*
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
        }*/
        /*
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
        }*/
    }
}
