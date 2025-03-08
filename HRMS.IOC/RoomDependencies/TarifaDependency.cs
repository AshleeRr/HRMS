using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Application.Services.RoomServices;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.ServiceValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Infraestructure.Logging;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.RoomDependencies;

public static class TarifaDependency
{
    public static IServiceCollection AddTarifaDependecy(this IServiceCollection services)
    {
        services.AddScoped<ITarifaRepository, TarifaRepository>();
        services.AddScoped<IValidator<Tarifas>, TarifasValidator>();
        services.AddScoped<ITarifaService, TarifaServices>();
        services.AddScoped<ILoggingServices, LoggingServices>();
        services.AddScoped<ICategoryRepository, CategoriaRepository>();
        return services;
    }
    
}