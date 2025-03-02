namespace HRMS.APIs;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuración de servicios
        ConfigureServices(builder.Services, builder.Configuration);
        var app = builder.Build();


        // Configuración del middleware
        ConfigureMiddleware(app, app.Environment);

        app.Run();
    }
    
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //services
            /*
            .AddPersistenceServices(configuration)
            .AddApplicationServices()
            .AddApiServices()
            .AddSwaggerConfiguration()
            .AddCorsConfiguration();*/
    }

    private static void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env)
    {
     //   app.UseCustomMiddleware(env);
        app.MapControllers();
    }
}
