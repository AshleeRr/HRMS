using WebApi.Models.UsersModels;

namespace WebApi.Interfaces.IUsersServices
{
    public interface IUserRepository : IGenericInterface<UserModel> 
    {
        Task<List<UserModel>> GetUsersByName(string nombreCompleto);
        Task<UserModel> GetUserByEmailAsync(string correo);
        Task<UserModel> GetUserByDocumentAsync(string documento);
        Task<List<UserModel>> GetUsersByTypeDocumentAsync(string tipoDocumento);
    }
}
