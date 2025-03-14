using HRMS.IOC.ReservationDepedencies;
using HRMS.IOC.RoomDependencies;
using HRMS.IOC.ServicesDependency;
using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<HRMSContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel")));
            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddPisoDependency();
            builder.Services.AddCategoryDependency();
            builder.Services.AddEstadoHabitacionDependency();
            builder.Services.AddRoomDependency();
            builder.Services.AddTarifaDependecy();
            builder.Services.AddServicioDependencies();
            builder.Services.AddReceptionDependencies();
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
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
