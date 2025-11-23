using City_Bus_Management_System.DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Service_Layer.ServiceRegistration
{
    public static class DataBaseConfigurationServiceRegistration
    {
        public static IServiceCollection AddDataBaseConfiguration(this IServiceCollection services,IConfigurationRoot config)
        {            
            var Connectionstring = config.GetSection("constr").Value;

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    Connectionstring,
                    sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(60); // 60 seconds

                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    });

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            return services;
        }
    }
}
