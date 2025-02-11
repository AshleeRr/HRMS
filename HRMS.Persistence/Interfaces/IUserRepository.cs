using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces
{
    public interface IUserRepository : IBaseRepository<Users, int>
    {
        //metodos exlucisvos de la entidad
    }
}
