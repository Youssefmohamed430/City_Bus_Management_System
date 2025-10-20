using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.ServiceRegistration
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddJwtAuthentication(config);
            services.AddScopedService();
            services.AddAppConfigurations(config);
            services.AddDataBaseConfiguration();
            services.AddCorsPolicy();
            services.AddRateLimiter();

            return services;
        }

    }
}
