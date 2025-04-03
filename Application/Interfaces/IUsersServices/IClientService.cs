using HRMS.Application.Base;
using HRMS.Application.DTOs.UsersDTOs.ClientDTOs;
using HRMS.Application.DTOs.UsersDTOs.UserDTOs;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;

namespace HRMS.Application.Interfaces.IUsersServices
{
    public interface IClientService : IBaseServices<SaveClientDTO, UpdateUserClientDTO, RemoveUserClientDTO>
    {
        Task<OperationResult> UpdatePasswordAsync(int id, string nuevaClave);
        Task<OperationResult> UpdateCorreoAsync(int id, string nuevoCorreo);
        Task<OperationResult> UpdateNombreCompletoAsync(int id, string nuevoNombreCompleto);
        //Task<Client> GetClientByUserIdAsync(int idUsuario);
        Task<OperationResult> GetClientByUserIdAsync(int idUsuario);
        Task<OperationResult> UpdateTipoDocumentoAndDocumentoAsync(int id, string tipoDocumento, string documento);

    }
}
