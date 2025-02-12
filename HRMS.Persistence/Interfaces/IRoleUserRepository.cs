using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces
{
    public interface IRoleUserRepository : IBaseRepository<UserRole, int>
    {
        Task<UserRole> GetRoleByDescription(string description);
        Task<OperationResult> UpdateDescription(int idRolUsuario, string descripcion);
    }
}
