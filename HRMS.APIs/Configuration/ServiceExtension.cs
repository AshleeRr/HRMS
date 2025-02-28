using HRMS.IOC.ReservationDepedencies;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IAuditRepository;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Persistence.Repositories.AuditRepository;
using HRMS.Persistence.Repositories.ClientRepository;
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
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
        services.AddReceptionDependencies();
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