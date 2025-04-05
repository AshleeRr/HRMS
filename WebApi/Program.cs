using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Repositories;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            // Configuración de la API
            string apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7175/api";
            
            builder.Services.AddSingleton<IApiClient>(provider => new ApiClient(apiBaseUrl));
            
            // Registrar los repositorios como servicios con ámbito (scoped)
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

            // Agregar soporte para HttpClient factory
            builder.Services.AddHttpClient();

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}