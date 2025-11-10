
using Data_Access_Layer;
using Service_Layer.BackgroundJobs;
using Service_Layer.Services;

namespace Service_Layer.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddService(this IServiceCollection services)
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
            services.AddSingleton<IBookingService, BookingService>();
            services.AddSingleton<ITrackingService, TrackingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IBackgroundJobService, BackgroundJobService>();
            services.AddScoped<JWTService>();

            return services;
        }
    }
}
