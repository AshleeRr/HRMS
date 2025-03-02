using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.ClientValidations;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Persistence.Repositories.UsersRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.ClientDependencies
{
    public static class ClientDependency
    {
        public static IServiceCollection AddClientDependencies(this IServiceCollection services)
        {
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IValidator<Client>, ClientValidator>();
            return services;
        }
    }
}
