using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.IOC.RoomDependency;

public static class CategoriaDependency
{
    public static IServiceCollection AddServiceCollection(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoriaRepository>();
        services.AddScoped<IValidator<Categoria>, CategoriaValidator>();
        return services;
    }
}