using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.ServiceValidations;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Interfaces.IServicioRepository;
using HRMS.Persistence.Repositories.ServiciosRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.ServicesDependency;

public static class ServicioDependency
{
    public static IServiceCollection AddServicioDependencies(this IServiceCollection services)
    {
        services.AddScoped<IValidator<Servicios>, ServiciosValidator>();
        services.AddScoped<IServicioRepository, ServicioRepository>();
        return services;
    }
}