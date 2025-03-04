using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IUserRoleRepository : IBaseRepository<UserRole, int>
    {
        Task<UserRole> GetRoleByDescriptionAsync(string descripcion);
        Task<UserRole> GetRoleByNameAsync(string rolNombre);
    }
}
