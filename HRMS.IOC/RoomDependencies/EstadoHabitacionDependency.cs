using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Application.DTOs.RoomManagementDto.Validations;
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

public static class EstadoHabitacionDependency
{
    public static IServiceCollection AddEstadoHabitacionDependency(this IServiceCollection services )
    {
        services.AddScoped<IEstadoHabitacionRepository, EstadoHabitacionRepository>();
        services.AddScoped<IValidator<EstadoHabitacion>, EstadoHabitacionValidator>();
        services.AddScoped<IEstadoHabitacionService, EstadoHabitacionService>();
        services.AddScoped<IValidator<CreateEstadoHabitacionDto>, EstadoHabitacionServiceValidator>();
        services.AddScoped<ILoggingServices, LoggingServices>();
        return services;
    }
}