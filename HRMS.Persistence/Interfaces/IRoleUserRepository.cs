using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces
{
    public interface IRoleUserRepository : IBaseRepository<UserRole, int>
    {
    }
}
