using WebApi.Models.UsersModels;

namespace WebApi.Interfaces.IUsersServices
{
    public interface IUserRoleRepository : IGenericInterface<UserRoleModel>
    {
        Task<UserRoleModel> GetRoleByDescription(string descripcion);
        Task<UserRoleModel> GetRoleByName(string nombre);
        Task<List<UserModel>> GetUsersByRole (int id);
    }
}
