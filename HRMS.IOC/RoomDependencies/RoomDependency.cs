using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Base.Validator.ServiceValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.RoomDependencies;

public static class CategoriaDependency
{
    public static IServiceCollection AddRoomCollection(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoriaRepository>();
        services.AddScoped<IValidator<Categoria>, CategoriaValidator>();
        
        services.AddScoped<IHabitacionRepository, HabitacionRepository>();
        services.AddScoped<IValidator<Habitacion>, HabitacionValidator>();
        
        services.AddScoped<IPisoRepository, PisoRepository>();
        services.AddScoped<IValidator<Piso>, PisoValidator>();
        
        services.AddScoped<ITarifaRepository, TarifaRepository>();
        services.AddScoped<IValidator<Tarifas>, TarifasValidator>();
        return services;
    }
}