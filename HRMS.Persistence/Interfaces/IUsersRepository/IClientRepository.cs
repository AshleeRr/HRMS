using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IClientRepository : IBaseRepository<Client, int>
    {
        Task<OperationResult> GetClientByEmailAsync(string correo);
        Task<OperationResult> GetClientByDocumentAsync(string documento);
        Task<Client> GetClientByUserIdAsync(int idUsuario);
        Task<OperationResult> GetClientsByTypeDocumentAsync(string tipoDocumento);
    }
}
