using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IUserRoleRepository : IBaseRepository<UserRole, int>
    {
        Task<OperationResult> GetRoleByDescriptionAsync(string descripcion);
        Task<OperationResult> GetRoleByNameAsync(string rolNombre);
        Task<OperationResult> GetUsersByUserRoleIdAsync(int id);
    }
}
