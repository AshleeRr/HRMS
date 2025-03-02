using HRMS.IOC.ReservationDepedencies;
using HRMS.IOC.AuditDependencies;
using HRMS.IOC.ClientDependencies;
using HRMS.IOC.RoomDependency
using HRMS.IOC.UserDependencies;
using HRMS.IOC.UserRoleDependencies;

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
            builder.Services.AddReceptionDependencies()
                               .AddUserDependencies()
                               .AddUserRoleDependencies()
                               .AddClientDependencies()
                               .AddAuditDependencies()
                               .AddServiceCollection();

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