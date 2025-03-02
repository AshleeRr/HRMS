using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IUserRepository : IBaseRepository<User, int>
    {
        Task<OperationResult> GetUsersByUserRoleIdAsync(int id);
        Task<List<User>> GetUsersByNameAsync(string nombreCompleto);
       // Task<OperationResult> UpdatePasswordAsync(int idUsuario, string nuevaClave);
      //  Task<User> AuthenticateUserAsync(string correo, string clave);
       // Task<OperationResult> UpdateEstadoAsync(User usuario, bool nuevoEstado);
        Task<User> GetUserByEmailAsync(string correo);
    }
}
