using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IUserRoleRepository : IBaseRepository<UserRole, int>
    {
        Task<UserRole> GetRoleByDescriptionAsync(string descripcion);
       // Task<OperationResult> UpdateDescriptionAsync(int idRolUsuario, string nuevaDescripcion);
       // Task<OperationResult> AsignDefaultRoleAsync(int idUsuario);
        //Task<OperationResult> AsignRolUserAsync(int idUsuario, int idRolUsuario);
    }
}
