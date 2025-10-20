using City_Bus_Management_System.Services;
using Core_Layer;
using Data_Access_Layer;
using Microsoft.Extensions.DependencyInjection;
using Service_Layer.IServices;
using Service_Layer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.ServiceRegistration
{
    public static class ScopedServiceRegistration
    {
        public static IServiceCollection AddScopedService(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IBusService, BusService>();
            services.AddScoped<ITripService, TripService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IStationService, StationService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}
