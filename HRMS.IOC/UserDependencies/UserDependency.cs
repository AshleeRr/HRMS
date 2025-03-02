using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.UserValidations;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Persistence.Repositories.UsersRepository;
using Microsoft.Extensions.DependencyInjection;

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
