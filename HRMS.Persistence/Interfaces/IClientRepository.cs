using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces
{
    public interface IClientRepository : IBaseRepository<Client, int>
    {
        Task<Client> GetClientByClientId(int IdCliente);
        Task<Client> GetClientByCorreo(string correo);
        Task<List<Client>> GetClientsByDocument(string documento);
    }
}
