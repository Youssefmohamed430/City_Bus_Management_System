
using Data_Access_Layer;
using Service_Layer.Services;

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
            services.AddScoped<ITrackingService, TrackingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<JWTService>();

            return services;
        }
    }
}
