
namespace Service_Layer.ServiceRegistration
{
    public static class ConfigurationServiceRegistration
    {
        public static IServiceCollection AddAppConfigurations(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JWT>(config.GetSection("JWT"));

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
            });

            return services;
        }
    }
}
