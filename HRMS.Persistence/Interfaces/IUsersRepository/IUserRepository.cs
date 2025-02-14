using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IUserRepository : IBaseRepository<Users, int>
    {
        Task<OperationResult> GetUsersByUserRolId(int idUsuario);
        Task<Users> GetUserByName(string nombreCompleto);
        Task<OperationResult> UpdatePassword(int idUsuario, string nuevaClave);
    }
}
