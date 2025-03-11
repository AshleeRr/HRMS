using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IUserRepository : IBaseRepository<User, int>
    {
        Task<List<User>> GetUsersByNameAsync(string nombreCompleto);
        Task<User> GetUserByDocumentAsync(string documento);
        Task<List<User>> GetUsersByTypeDocumentAsync(string tipoDocumento);
        Task<User> GetUserByEmailAsync(string correo);
    }
}
