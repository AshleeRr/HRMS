using HRMS.IOC.UsersDependencies;
using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<HRMSContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel"));
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddClientDependencies();
            builder.Services.AddUserDependencies();
            builder.Services.AddUserRoleDependencies();


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
