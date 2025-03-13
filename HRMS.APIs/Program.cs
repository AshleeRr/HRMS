using HRMS.IOC.AuditDependencies;
using HRMS.IOC.UsersDependencies;
using HRMS.IOC.ReservationDepedencies;
using HRMS.IOC.RoomDependencies;
using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HRMS.APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<HRMSContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel"));
            });
            builder.Services.AddReceptionDependencies();

            // añadir las dependencias
            builder.Services.AddClientDependencies();
            builder.Services.AddUserDependencies();
            builder.Services.AddUserRoleDependencies();
            builder.Services.AddAuditDependencies();

            //Dependencias de Habitaciones
            builder.Services.AddPisoDependency();
            builder.Services.AddCategoryDependency();
            builder.Services.AddEstadoHabitacionDependency();
            builder.Services.AddRoomDependency();
            builder.Services.AddTarifaDependecy();
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

            app.MapControllers();

            app.Run();
        }
    }
}