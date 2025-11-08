using System.Collections.Concurrent;

namespace Service_Layer.Services
{
    public class TrackingService(IServiceScopeFactory _scopeFactory, ILogger<TrackingService> logger) : ITrackingService
    {
        public ConcurrentQueue<Station> StationsOrder = new ConcurrentQueue<Station>();
        public List<Passenger> bookingsFrom = null!;
        public List<Passenger> bookingsTo = null!;
        private int lastNotificationDuration = 0 ;
        
        public async Task GetNextStationAtTrip(int tripId, double buslng, double buslat)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                
                if (StationsOrder.IsEmpty)
                    FillQueue(tripId);

                if (StationsOrder.TryPeek(out Station? nextStation))
                {
                    var coor = await routeService.CalcDistanceToDistnation(buslng, buslat, nextStation.Longitude, nextStation.Latitude);
                    int Duration = (int)coor.Duration;

                    logger.LogInformation($"Duration to next station: {Duration} minutes");

                    int? newDuration = Duration switch
                    {
                        <= 15 and > 10 => 15 , <= 10 and > 5 => 10,
                        <= 5 and > 1 => 5 , _ => null
                    };

                    bool shouldNotify = newDuration is not null && newDuration != lastNotificationDuration;

                    if (shouldNotify)
                    {
                        lastNotificationDuration = newDuration.Value;

                        logger.LogInformation($"Sending notifications for Duration: {lastNotificationDuration}");

                        await NotifybookingsAsFrom(tripId, notificationService, nextStation);

                        await NotifybookingAsTo(tripId, notificationService, nextStation);
                    }

                    if (Duration <= 1)
                    {
                        logger.LogInformation($"Reached station {nextStation.Name}, moving to next");
                        
                        Reset();
                    }
                }
            }
        }

        private void Reset()
        {
            StationsOrder.TryDequeue(out _);
            bookingsFrom = null!;
            bookingsTo = null!;
            lastNotificationDuration = 0;
        }

        private async Task NotifybookingAsTo(int tripId, INotificationService notificationService, Station nextStation)
        {
            if (bookingsTo == null)
                GetBookingsTo(tripId, nextStation);

            if (bookingsTo != null && bookingsTo.Count > 0)
            {
                logger.LogInformation($"Notifying {bookingsTo.Count} passengers alighting at station {nextStation.Name}");
                await notificationService.NotifStationApproaching(bookingsTo, lastNotificationDuration, false);
            }
        }

        private async Task NotifybookingsAsFrom(int tripId, INotificationService notificationService, Station nextStation)
        {
            if (bookingsFrom == null)
                GetBookingsFrom(tripId, nextStation);

            if (bookingsFrom != null && bookingsFrom.Count > 0)
            {
                logger.LogInformation($"Notifying {bookingsFrom.Count} passengers boarding at station {nextStation.Name}");
                await notificationService.NotifStationApproaching(bookingsFrom, lastNotificationDuration, true);
            }
        }

        private void GetBookingsTo(int tripId, Station nextStation)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                bookingsTo = unitOfWork.GetRepository<Booking>()
                                   .FindAll(b => b.TripId == tripId
                                              && b.StationToId == nextStation.Id
                                              && b.Status == "Booked",
                                              new string[] { "passenger" })
                                   .Select(b => b.passenger!)
                                   .ToList();
            }
        }

        private void GetBookingsFrom(int tripId, Station nextStation)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                bookingsFrom = unitOfWork.GetRepository<Booking>()
                                     .FindAll(b => b.TripId == tripId
                                                && b.StationFromId == nextStation.Id
                                                && b.Status == "Booked",
                                                new string[] { "passenger" })
                                     .Select(b => b.passenger!)
                                     .ToList();
            }
        }

        private void FillQueue(int tripId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var stations = unitOfWork.GetRepository<Route>()
                                .FindAll(r => r.TripId == tripId && !r.IsDeleted, new string[] { "station" })
                                .OrderBy(r => r.Order)
                                .Select(r => r.station)
                                .Skip(1)
                                .ToList();

                foreach (var station in stations)
                    StationsOrder.Enqueue(station);
            } 
        }
    }
}
