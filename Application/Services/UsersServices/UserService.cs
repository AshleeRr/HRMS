using HRMS.Application.DTOs.UserDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.UsersServices
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository,
                                ILogger<UserService> logger, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public Task<User> AuthenticateUserAsync(string correo, string clave)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Remove(RemoveUserDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Save(SaveUserDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Update(UpdateUserDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> UpdatePasswordAsync(int idUsuario, string nuevaClave)
        {
            throw new NotImplementedException();
        }

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
    }
}
