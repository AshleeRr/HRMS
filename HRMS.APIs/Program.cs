using HRMS.IOC.AuditDependencies;
using HRMS.IOC.UsersDependencies;
using HRMS.IOC.ReservationDepedencies;
using HRMS.Persistence.Context;
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

            builder.Services.AddControllers();
            builder.Services.AddDbContext<HRMSContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel"));
            });
            builder.Services.AddReceptionDependencies();

            // a�adir las dependencias
            builder.Services.AddClientDependencies();
            builder.Services.AddUserDependencies();
            builder.Services.AddUserRoleDependencies();
            builder.Services.AddAuditDependencies();

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