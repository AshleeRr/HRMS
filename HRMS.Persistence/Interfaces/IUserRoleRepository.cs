using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces
{
    public interface IUserRoleRepository : IBaseRepository<UserRole, int>
    {
        Task<OperationResult> UpdateDescription(int idRolUsuario, string nuevaDescripcion);
    }
}
