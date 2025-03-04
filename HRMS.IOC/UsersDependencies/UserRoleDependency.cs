﻿using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.UsersValidations;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Persistence.Repositories.UsersRepository;
using Microsoft.Extensions.DependencyInjection;
namespace HRMS.IOC.UsersDependencies
{
    public static class UserRoleDependency
    {
        public static IServiceCollection AddUserRoleDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IValidator<UserRole>, UserRoleValidator>();
            return services;
        }
    }
}
