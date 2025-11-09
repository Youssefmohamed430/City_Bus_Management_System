using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.ServiceRegistration
{
    public static class SerilogConfigs
    {
        public static IServiceCollection AddSerilogConfigs(this IServiceCollection services, IConfigurationRoot config)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            return services;
        }
    }
}
