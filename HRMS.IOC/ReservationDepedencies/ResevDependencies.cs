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
            return services;
        }
    }
}
