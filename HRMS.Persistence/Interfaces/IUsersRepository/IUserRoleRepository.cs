using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IUserRoleRepository : IBaseRepository<UserRole, int>
    {
        Task<UserRole> GetRoleByDescription(string descripcion);
        Task<OperationResult> UpdateDescription(int idRolUsuario, string nuevaDescripcion);
    }
}
