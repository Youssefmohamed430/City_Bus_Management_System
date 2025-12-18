using City_Bus_Management_System.DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Service_Layer.ServiceRegistration
{
    public static class DataBaseConfigurationServiceRegistration
    {
        public static IServiceCollection AddDataBaseConfiguration(this IServiceCollection services, IConfigurationRoot config)
        {
            //var Connectionstring = Environment.GetEnvironmentVariable("Constr");
            var Connectionstring = config.GetConnectionString("Constr");

            services.AddDbContext<AppDbContext>(options =>
            { options.UseSqlServer(Connectionstring);});

            return services;
        }
    }
}
