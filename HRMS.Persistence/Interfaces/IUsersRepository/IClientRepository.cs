using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IClientRepository : IBaseRepository<Client, int>
    {
        Task<Client> GetClientByEmailAsync(string correo);
        Task<Client> GetClientByDocumentAsync(string documento);
        // obtener reservas de un cliente (verhistorial)
        Task<List<Client>> GetClientsByTypeDocumentAsync(string tipoDocumento);
    }
}
