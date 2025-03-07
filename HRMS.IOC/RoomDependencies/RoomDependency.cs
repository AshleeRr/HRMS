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

public static class RoomDependency
{
    public static IServiceCollection AddRoomDependency(this IServiceCollection services)
    {
        services.AddScoped<IHabitacionRepository, HabitacionRepository>();
        services.AddScoped<IValidator<Habitacion>, HabitacionValidator>();
        services.AddScoped<IHabitacionService, HabitacionServices>();
        services.AddScoped<ILoggingServices, LoggingServices>();
        return services;
    }
}