using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;
using System.Threading.Tasks;

namespace HRMS.Persistence.Interfaces.Users
{
    public interface IUserRepository : IBaseRepository<User, int>
    {
        Task<OperationResult> GetUserByUserId(int idUsuario);
        Task<OperationResult> GetUsersByUserRolId(int idUsuario);
        Task<User> GetUserByName(string nombreCompleto);
        Task<OperationResult> UpdatePassword(int idUsuario, string nuevaClave);
    }
}
