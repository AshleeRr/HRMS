using HRMS.IOC.ReservationDepedencies;
using HRMS.IOC.AuditDependencies;
using HRMS.IOC.AuditDependencies;
using HRMS.IOC.UsersDependencies;
using HRMS.IOC.RoomDependencies;
using Microsoft.EntityFrameworkCore;
using HRMS.Persistence.Context;

namespace HRMS.APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddTransient<IHabitacionRepository, HabitacionRepository>();
            builder.Services.AddControllers();
            builder.Services.AddDbContext<HRMSContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel"));
            });
            builder.Services.AddReceptionDependencies()
                               .AddUserDependencies()
                               .AddUserRoleDependencies()
                               .AddClientDependencies()
                               .AddAuditDependencies()
                               .AddRoomCollection();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}