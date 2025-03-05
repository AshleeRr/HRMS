using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Application.Services.RoomServices;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Domain.Repository;
using HRMS.Infraestructure.Logging;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.Reserv;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.RoomDependencies;

public static class PisoDependency
{
    public static IServiceCollection AddPisoDependency(this IServiceCollection services)
    {
        services.AddScoped<IPisoRepository, PisoRepository>();
        services.AddScoped<IValidator<Piso>, PisoValidator>();
        services.AddScoped<IHabitacionRepository, HabitacionRepository>();  
        services.AddScoped<IReservationRepository, ReservationRepository>(); 
        services.AddScoped<ILoggingServices, LoggingServices>();

        return services;
    }
}