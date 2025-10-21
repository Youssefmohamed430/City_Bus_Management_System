using City_Bus_Management_System.DataLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace Service_Layer.ServiceRegistration
{
    public static class DataBaseConfigurationServiceRegistration
    {
        public static IServiceCollection AddDataBaseConfiguration(this IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var Connectionstring = config.GetSection("constr").Value;

            services.AddDbContextPool<AppDbContext>(options =>
                options.UseSqlServer(Connectionstring)
            );

            return services;
        }
    }
}
