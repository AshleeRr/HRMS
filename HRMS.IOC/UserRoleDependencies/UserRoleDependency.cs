using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Persistence.Repositories.ClientRepository;
using Microsoft.Extensions.DependencyInjection;
using MyValidator.Validator;

namespace HRMS.IOC.UserRoleDependencies
{
    public static class UserRoleDependency
    {
        public static IServiceCollection AddUserRoleDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IValidator<UserRole>, UserRoleValidator>();
            return services;
        }
    }
}
