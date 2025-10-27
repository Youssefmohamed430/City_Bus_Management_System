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
            .Map(dest => dest.BusType, src => src.bus.BusType)
            .Map(dest => dest.DriverName, src => src.driver.User.Name)
            .Map(dest => dest.From, src => src.trip != null ? src.trip.From : null)
            .Map(dest => dest.To, src => src.trip != null ? src.trip.To : null);

            TypeAdapterConfig<Route, RouteDTO>
            .NewConfig()
            .Map(dest => dest.From, src => src.trip.From)
            .Map(dest => dest.To, src => src.trip.To)
            .Map(dest => dest.StationName, src => src.station.Name);

            TypeAdapterConfig<Route, StationRouteDTO>
            .NewConfig()
            .Map(dest => dest.From, src => src.trip.From)
            .Map(dest => dest.To, src => src.trip.To)
            .Map(dest => dest.StationName, src => src.station.Name)
            .Map(dest => dest.Latitude, src => src.station.Latitude)
            .Map(dest => dest.Longitude, src => src.station.Longitude);

            TypeAdapterConfig<Wallet, WalletDTO>
            .NewConfig()
            .Map(dest => dest.Name, src => src.passenger.User.Name);

            TypeAdapterConfig<Booking, BookingDTO>
            .NewConfig()
            .Map(dest => dest.passengerName, src => src.passenger.User.Name)
            .Map(dest => dest.TicketId, src => src.TicketId)
            .Map(dest => dest.BusType, src => src.Ticket.BusType)
            .Map(dest => dest.Price, src => src.Ticket.Price)
            .Map(dest => dest.TripId, src => src.TripId)
            .Map(dest => dest.StationFromId, src => src.StationFromId)
            .Map(dest => dest.StationToId, src => src.StationToId)
            .Map(dest => dest.passengerName, src => src.passenger.User.Name);

            TypeAdapterConfig<BookingDTO, Booking>
            .NewConfig()
            .Map(dest => dest.TicketId, src => src.TicketId)
            .Map(dest => dest.TripId, src => src.TripId)
            .Map(dest => dest.passengerId, src => src.passengerId);



            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
