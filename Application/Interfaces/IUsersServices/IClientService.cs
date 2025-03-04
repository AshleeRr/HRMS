using HRMS.Application.Base;
using HRMS.Application.DTOs.ClientDTOs;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.IUsersServices
{
    public interface IClientService : IBaseServices<SaveClientDTO, UpdateClientDTO, RemoveClientDTO>
    {
        Task<OperationResult> UpdateTipoDocumentoAndDocumentoAsync(int idCliente, int idTipoDocumento, string documento);
    }
}
