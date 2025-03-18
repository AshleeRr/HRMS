using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Application.Services.UsersServices;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Moq;


namespace HRMS.Application.Test.UsersTests
{
    public class UnitTestUserRoleService
    {
        private readonly Mock<IUserRoleRepository> _userRoleRepository;
        private readonly UserRoleService _userService;
    }
}
