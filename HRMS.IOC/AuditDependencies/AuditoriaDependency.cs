using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.AuditValidations;
using HRMS.Domain.Entities.Audit;
using HRMS.Persistence.Interfaces.IAuditRepository;
using HRMS.Persistence.Repositories.AuditRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.AuditDependencies
{
    public static class AuditoriaDependency
    {
        public static IServiceCollection AddAuditDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<IValidator<Auditoria>, AuditoriaValidator>();
            return services;
        }
    }
}

