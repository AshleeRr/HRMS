using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Persistence.Repositories.ClientRepository;
using Microsoft.Extensions.DependencyInjection;
using MyValidator.Validator;

namespace HRMS.IOC.UserDependencies
{
    public static class UserDependency
    {
        public static IServiceCollection AddUserDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IValidator<User>, UserValidator>();
            return services;
        }
    }
}
