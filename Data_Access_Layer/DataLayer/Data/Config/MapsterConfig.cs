using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<Schedule, ScheduleDTO>
            .NewConfig()
            .Map(dest => dest.BusCode, src => src.bus.BusCode)
            .Map(dest => dest.DriverName, src => src.driver.User.Name)
            .Map(dest => dest.From, src => src.trip != null ? src.trip.From : null)
            .Map(dest => dest.To, src => src.trip != null ? src.trip.To : null);

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
