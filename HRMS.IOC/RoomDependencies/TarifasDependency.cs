using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.ServiceValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.RoomDependency;

public static class TarifasDependency
{
    public static IServiceCollection AddServiceCollection(this IServiceCollection services)
    {
        services.AddScoped<ITarifaRepository, TarifaRepository>();
        services.AddScoped<IValidator<Tarifas>, TarifasValidator>();
        return services;
    }
}