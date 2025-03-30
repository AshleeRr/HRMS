using HRMS.Application.Base;
using HRMS.Application.DTOs.UsersDTOs.UserRoleDTOs;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.IUsersServices
{
    public interface IUserRoleService : IBaseServices<SaveUserRoleDTO, UpdateUserRoleDTO, RemoveUserRoleDTO>
    {
        Task<OperationResult> UpdateDescriptionAsync(int idRolUsuario, string nuevaDescripcion);
        Task<OperationResult> UpdateNameAsync(int idRolUsuario, string nuevoNombre);

    }
}
