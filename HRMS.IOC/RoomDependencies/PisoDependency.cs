using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.RoomDependency;

public static class PisoDependency
{
    public static IServiceCollection AddServicesCollection(this IServiceCollection services)
    {
        services.AddScoped<IPisoRepository, PisoRepository>();
        services.AddScoped<IValidator<Piso>, PisoValidator>();
        return services;
    }
}