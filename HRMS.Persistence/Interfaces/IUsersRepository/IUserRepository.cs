using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IUserRepository : IBaseRepository<User, int>
    {
        Task<OperationResult> GetUsersByUserRoleIdAsync(int id);
        Task<List<User>> GetUsersByNameAsync(string nombreCompleto);
        Task<User> GetUserByEmailAsync(string correo);
    }
}
