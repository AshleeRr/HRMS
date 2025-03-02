using HRMS.Application.Base;
using HRMS.Application.DTOs.UserRoleDTOs;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.IUsersServices
{
    public interface IUserRoleService : IBaseServices<SaveUserRoleDTO, UpdateUserRoleDTO, RemoveUserRoleDTO>
    {
        Task<OperationResult> AsignDefaultRoleAsync(int idUsuario);
        Task<OperationResult> UpdateDescriptionAsync(int idRolUsuario, string nuevaDescripcion);
        Task<OperationResult> AsignRolUserAsync(int idUsuario, int idRolUsuario);

    }
}
