using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.ServiceRegistration
{
    public static class RemainingAppConfig
    {
        public static IServiceCollection AddRemainingAppConfigs(this IServiceCollection services)
        {
            services.AddLogging(cfg => cfg.AddDebug());

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true; // Helpful for debugging
            });

            services.AddEndpointsApiExplorer();

            services.AddMemoryCache();

            return services;
        }
    }
}
