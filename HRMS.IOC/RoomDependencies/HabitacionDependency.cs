using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.RoomDependency;

public static class HabitacionDependency
{
    public static IServiceCollection AAddServiceCollection(this IServiceCollection services)
    {
        services.AddScoped<IHabitacionRepository, HabitacionRepository>();
        services.AddScoped<IValidator<Habitacion>, HabitacionValidator>();
        return services;
    }
}