
namespace Service_Layer.ServiceRegistration
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAppConfigurations(config);   
            services.AddDataBaseConfiguration();   
            services.AddIdentityService();          
            services.AddJwtAuthentication(config);   
            services.AddScopedService();                  
            services.AddCorsPolicy();                
            services.AddRateLimiter();               


            return services;
        }

    }
}
