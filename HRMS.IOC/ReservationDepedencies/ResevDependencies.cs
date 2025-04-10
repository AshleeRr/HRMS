﻿using HRMS.Application.DTOs.Reservation_2023_0731;
using HRMS.Application.DTOs.Reservation_2023_0731.ReservDtosValidator;
using HRMS.Application.Interfaces.Reservation_2023_0731;
using HRMS.Application.Services.Reservation_2023_0731;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.ReservationValidator;
using HRMS.Domain.Entities.Reservations;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Domain.Repository;
using HRMS.Infraestructure.Logging;
using HRMS.Infraestructure.Notification;
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
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IValidator<ReservationAddDTO>, ReservAddDtoValidator>();
            services.AddScoped<ILoggingServices, LoggingServices>();
            services.AddScoped<INotificationService, NotificationServiceFake>();
            return services;
        }
    }
}
