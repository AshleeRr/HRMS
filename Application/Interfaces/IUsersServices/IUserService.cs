using HRMS.Application.Base;
using HRMS.Application.DTOs.UserDTOs;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.IUsersServices
{
    public interface IUserService : IBaseServices<SaveUserDTO, UpdateUserDTO, RemoveUserDTO>
    {
        Task<OperationResult> UpdatePasswordAsync(int idUsuario, string nuevaClave);
        Task<OperationResult> UpdateCorreoAsync(int idUsuario, string nuevoCorreo);
        Task<OperationResult> UpdateNombreCompletoAsync(int idUsuario, string nuevoNombreCompleto);
        Task<OperationResult> UpdateUserRoleToUserAsync(int idUsuario, int idUserRole);
        Task<OperationResult> UpdateTipoDocumentoAndDocumentoAsync(int idUsuario, string tipoDocumento, string documento);
    }
}
