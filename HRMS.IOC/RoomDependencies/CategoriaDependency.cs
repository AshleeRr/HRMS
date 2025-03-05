using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Application.Services.RoomServices;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Infraestructure.Logging;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.RoomDependencies;

public static class CategoriaDependency
{
    public static IServiceCollection AddCategoryDependency(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoriaRepository>();
        services.AddScoped<IValidator<Categoria>, CategoriaValidator>();
        services.AddScoped<ILoggingServices, LoggingServices>();
        services.AddScoped<ICategoryService, CategoriaServices>();
    
        return services;
    }
}