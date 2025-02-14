using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;
using HRMS.Domain.Base;

namespace HRMS.Persistence.Interfaces.Users
{
    public interface IClientRepository : IBaseRepository<Client, int>
    {
        Task<Client> GetClientByClientId(int idCliente);
        Task<Client> GetClientByCorreo(string correo);
        Task<List<Client>> GetClientsByDocument(string documento);
    }
}
