using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.ReservationValidator;
using HRMS.Domain.Entities.Reservation;
using HRMS.Domain.Repository;
using HRMS.Persistence.Repositories.Reserv;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.ReservationDepedencies
{
    public static class ResevDependencies
    {
        public static IServiceCollection AddReceptionDependencies(this IServiceCollection services)
        {
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IValidator<Reservation>, ReservationValidator>();
            return services;
        }
    }
}
