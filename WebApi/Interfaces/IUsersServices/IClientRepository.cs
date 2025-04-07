using WebApi.Models.UsersModels;

namespace WebApi.Interfaces.IUsersServices
{
    public interface IClientRepository 
    {
        Task<ClientModel> GetClientByEmail(string email);
        Task<ClientModel> GetClientByDocument(string document);
        Task<List<ClientModel>> GetClientsByDocumentType(string tipoDocumento);
        Task<IEnumerable<ClientModel>> GetAllAsync();
        Task<ClientModel> GetByIdAsync(int id);
    }
}
