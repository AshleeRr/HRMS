using HRMS.Application.Base;
using HRMS.Application.DTOs.UserDTOs;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;

namespace HRMS.Application.Interfaces.IUsersServices
{
    public interface IUserService : IBaseServices<SaveUserDTO, UpdateUserDTO, RemoveUserDTO>
    {
        Task<OperationResult> UpdatePasswordAsync(int idUsuario, string nuevaClave);
        Task<User> AuthenticateUserAsync(string correo, string clave); // no segura de este metodo
    }
}
