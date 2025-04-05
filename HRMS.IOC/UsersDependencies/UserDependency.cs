using HRMS.Application.DTOs.UsersDTOs.UserDTOs;
using HRMS.Application.DTOs.UsersDTOs.ValidationsForSaveDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using HRMS.Application.Services.UsersServices;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.UsersValidations;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Infraestructure.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Persistence.Repositories.UsersRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.UsersDependencies
{
    public static class UserDependency
    {
        public static IServiceCollection AddUserDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IValidator<User>, UserValidator>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IValidator<SaveUserDTO>, UserServiceValidator>();
            services.AddScoped<ILoggingServices, LoggingServices>();
            //services.AddScoped<INotificationService, NotificationServiceFake>();
            return services;
        }
    }
}
