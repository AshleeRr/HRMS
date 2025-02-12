using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;
using System.Threading.Tasks;

namespace HRMS.Persistence.Interfaces
{
    public interface IUserRepository : IBaseRepository<Users, int>
    {
        Task<Users> GetUserByUserId(int idUsuario);
        Task<OperationResult> DeleteUserbyId(int idUsuario);
        Task<Users> GetUserByName(string nombreCompleto);
        Task<OperationResult> UpdatePassword(int idUsuario, string nuevaClave);
    }
}
