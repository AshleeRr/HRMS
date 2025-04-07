using HRMS.Domain.Base.Validator;
using HRMS.WebApi.Models.Reservation_2023_0731;
using WebApi.Adapters.Reservation;
using WebApi.ServicesInterfaces.Reservation;
using WebApi.Validators.Reservations;

namespace WebApi.IOC.Reservation
{
    public static class ReservationDependencies
    {
        public static IServiceCollection AddReceptionDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IReservationRepository, ReservationAdapter>();
            services.AddScoped<IValidator<ReservationAddDTO>, ReservationCreateValidator>();
            services.AddScoped<IValidator<ReservationUpdateDTO>, ReservationUpdateValidator>();
            services.AddHttpClient("Reservations", client =>
            {
                client.BaseAddress = new Uri($"{configuration["ApiBaseUrl"]}/Reservations/");
            });
            return services;
        }
    }
}
