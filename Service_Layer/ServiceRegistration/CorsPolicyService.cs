using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.ServiceRegistration
{
    public static class CorsPolicyService
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(corsOptions =>
                  corsOptions.AddPolicy("MyPolicy", CorsPolicy =>
                      CorsPolicy.WithOrigins(
                            "https://youssefmohamed430.github.io"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
            ));

            return services;
        }
    }
}
