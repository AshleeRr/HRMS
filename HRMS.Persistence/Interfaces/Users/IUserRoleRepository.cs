using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.Users
{
    public interface IUserRoleRepository : IBaseRepository<UserRole, int>
    {
        Task<UserRole> GetRoleByUserRolId(int idRolUsuario);
        Task<OperationResult> UpdateDescription(int idRolUsuario, string nuevaDescripcion);
    }
}
