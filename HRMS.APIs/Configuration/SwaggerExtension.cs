using Microsoft.OpenApi.Models;

namespace HRMS.APIs.Configuration;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "HRMS API",
                Version = "v1",
                Description = "Api para gestion hotelera",
                Contact = new OpenApiContact()
                {
                    Name = "HRMS",
                    Email = string.Join(", ", new[]
                    {
                        "ashleeramirezrosario@gmail.com", "ajeromepuente@gmail.com" 
                        ,"Supremeyunior008@gmail.com", "Cristopherxanderazadiaz19111@gmail.com"
                    })
                }
            });
        });
        return services;
    }
    
    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "HRMS API V1");
        });
        return app;
    }
}