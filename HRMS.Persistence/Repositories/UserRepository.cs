using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Base;
using HRMS.Persistence.Interfaces;

namespace HRMS.Persistence.Repositories
{
    public class UserRepository : BaseRepository<Users, int>, IUserRepository
    {

    }
}
