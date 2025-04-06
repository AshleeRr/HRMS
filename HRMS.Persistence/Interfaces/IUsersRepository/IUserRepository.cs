using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;

namespace HRMS.Persistence.Interfaces.IUsersRepository
{
    public interface IUserRepository : IBaseRepository<User, int>
    {
        Task<OperationResult> GetUsersByNameAsync(string nombreCompleto);
        Task<OperationResult> GetUserByDocumentAsync(string documento);
        Task<OperationResult> GetUsersByTypeDocumentAsync(string tipoDocumento);
        Task<OperationResult> GetUserByEmailAsync(string correo);
        Task<int> GetLastReferenceIDAsync();
        Task<bool> UserIDExistsAsync(int userID);


    }
}
