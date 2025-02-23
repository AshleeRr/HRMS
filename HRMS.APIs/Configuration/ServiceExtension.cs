using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.APIs.Configuration;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IHabitacionRepository, HabitacionRepository>();
        services.AddScoped<ICategoryRepository, CategoriaRepository>();
        services.AddScoped<IPisoRepository, PisoRepository>();
        services.AddScoped<IEstadoHabitacionRepository, EstadoHabitacionRepository>();
        //Añadir todos los servicios aqui
        return services;
    }
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HRMSContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DBHotel"));
        });
        
        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddAuthentication();
        services.AddAuthorization();
        
        return services;
    }
}