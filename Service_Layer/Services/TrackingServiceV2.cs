using Data_Access_Layer.DataLayer.DTOs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.Services
{
    public class TrackingServiceV2(IServiceScopeFactory _scopeFactory, ILogger<TrackingService> logger) : ITrackingService
    {
        public ConcurrentDictionary<Station,int> StationsOrder = new ConcurrentDictionary<Station,int>();
        private Dictionary<int, int> stationLastNotify = new();
        public async Task GetNextStationAtTrip(int tripId, double buslng, double buslat)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                if (StationsOrder.IsEmpty)
                    FillDic(tripId,unitOfWork);

                foreach(var station in StationsOrder.Keys)
                {
                    await HandleDuration(buslng, buslat, routeService, station);

                    int? newDuration = StationsOrder[station] switch
                    {
                        <= 15 and > 10 => 15,
                        <= 10 and > 5 => 10,
                        <= 5 and > 1 => 5,
                        _ => null
                    };

                    if (!stationLastNotify.ContainsKey(station.Id))
                        stationLastNotify[station.Id] = 0;

                    bool shouldNotify = newDuration is not null &&
                                        newDuration != stationLastNotify[station.Id];

                    if (shouldNotify)
                    {
                        stationLastNotify[station.Id] = newDuration.Value;
                        logger.LogInformation($"Sending notifications for Duration: {stationLastNotify[station.Id]}");
                        await NotifybookingsAsFrom(tripId, notificationService, station, unitOfWork);
                        await NotifybookingAsTo(tripId, notificationService, station, unitOfWork);
                    }

                    if (StationsOrder[station] <= 0)
                    {
                        StationsOrder.TryRemove(station, out _);
                        stationLastNotify.Remove(station.Id);
                        logger.LogInformation($"Bus has arrived at station {station.Name}, removing from tracking.");
                    }
                }
            }
        }

        private async Task HandleDuration(double buslng, double buslat, IRouteService routeService, Station station)
        {
            if (StationsOrder[station] == 0)
            {
                var coor = await routeService.CalcDistanceToDistnation(buslng, buslat, station.Longitude, station.Latitude);
                int Duration = (int)coor.Duration;
                StationsOrder[station] = Duration;
            }
            else
                StationsOrder[station]--;
        }

        private async Task NotifybookingAsTo(int tripId, INotificationService notificationService, Station station, IUnitOfWork unitOfWork)
        {
            var bookingsTo = GetBookingsTo(tripId, station,unitOfWork);

            if (bookingsTo != null && bookingsTo.Count > 0)
            {
                logger.LogInformation($"Notifying {bookingsTo.Count} passengers alighting at station {station.Name}");
                await notificationService.NotifStationApproaching(bookingsTo,stationLastNotify[station.Id], false);
            }
        }

        private List<Passenger> GetBookingsTo(int tripId, Station station,IUnitOfWork unitOfWork)
        {
             var bookingsTo = unitOfWork.GetRepository<Booking>()
                                .FindAll(b => b.TripId == tripId
                                           && b.StationToId == station.Id
                                           && b.Status == "Booked",
                                           new string[] { "passenger" })
                                .Select(b => b.passenger!)
                                .ToList();

             return bookingsTo;
        }

        private async Task NotifybookingsAsFrom(int tripId, INotificationService notificationService, Station station,IUnitOfWork unitOfWork)
        {
            var bookingsFrom = GetBookingsFrom(tripId, station, unitOfWork);    

            if (bookingsFrom != null && bookingsFrom.Count > 0)
            {
                logger.LogInformation($"Notifying {bookingsFrom.Count} passengers boarding at station {station.Name}");
                await notificationService.NotifStationApproaching(bookingsFrom,stationLastNotify[station.Id], true);
            }
        }

        private List<Passenger> GetBookingsFrom(int tripId, Station station,IUnitOfWork unitOfWork)
        {
                var bookingsFrom = unitOfWork.GetRepository<Booking>()
                                     .FindAll(b => b.TripId == tripId
                                                && b.StationFromId == station.Id
                                                && b.Status == "Booked",
                                                new string[] { "passenger" })
                                     .Select(b => b.passenger!)
                                     .ToList();

                return bookingsFrom;
        }

        private void FillDic(int tripId,IUnitOfWork unitOfWork)
        {
             var stations = unitOfWork.GetRepository<Route>()
                             .FindAll(r => r.TripId == tripId && !r.IsDeleted, new string[] { "station" })
                             .OrderBy(r => r.Order)
                             .Select(r => r.station)
                             .Skip(1)
                             .ToList();
            
             foreach (var station in stations)
                 StationsOrder.TryAdd(station,0);
        }
    }
}
