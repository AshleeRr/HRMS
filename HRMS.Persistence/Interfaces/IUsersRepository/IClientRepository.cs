using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IClientRepository : IBaseRepository<Client, int>
    {
        Task<Client> GetClientByCorreo(string correo);
        Task<List<Client>> GetClientsByDocument(string documento);
    }
}
