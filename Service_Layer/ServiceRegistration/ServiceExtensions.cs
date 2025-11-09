
namespace Service_Layer.ServiceRegistration
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            var appsettingsconfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddAppConfigurations(config);   
            services.AddDataBaseConfiguration(appsettingsconfig);   
            services.AddSerilogConfigs(appsettingsconfig);   
            services.AddIdentityService();          
            services.AddJwtAuthentication(config);   
            services.AddService();                  
            services.AddCorsPolicy();                
            services.AddRateLimiter();               


            return services;
        }

    }
}
