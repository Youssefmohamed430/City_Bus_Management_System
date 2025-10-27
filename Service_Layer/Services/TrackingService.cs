using System.Collections.Concurrent;

namespace Service_Layer.Services
{
    public class TrackingService(IUnitOfWork unitOfWork, IRouteService routeService, INotificationService notificationService, ILogger<TrackingService> logger) : ITrackingService
    {
        public ConcurrentQueue<Station> StationsOrder = new ConcurrentQueue<Station>();
        public List<Passenger> bookingsFrom = null!;
        public List<Passenger> bookingsTo = null!;
        private int lastNotificationDuration = 0;
        
        public async Task GetNextStationAtTrip(int tripId, double buslng, double buslat)
        {
            if (StationsOrder.IsEmpty)
                FillQueue(tripId);

            if (StationsOrder.TryPeek(out Station? nextStation))
            {
                var coor = await routeService.CalcDistanceToDistnation(buslng, buslat, nextStation.Longitude, nextStation.Latitude);
                int Duration = (int)coor.Duration;

                logger.LogInformation($"Duration to next station: {Duration} minutes");

                bool shouldNotify = false;

                if (Duration <= 15 && Duration > 10 && lastNotificationDuration != 15)
                {
                    shouldNotify = true;
                    lastNotificationDuration = 15;
                }
                else if (Duration <= 10 && Duration > 5 && lastNotificationDuration != 10)
                {
                    shouldNotify = true;
                    lastNotificationDuration = 10;
                }
                else if (Duration <= 5 && Duration > 1 && lastNotificationDuration != 5)
                {
                    shouldNotify = true;
                    lastNotificationDuration = 5;
                }

                if (shouldNotify)
                {
                    logger.LogInformation($"Sending notifications for Duration: {lastNotificationDuration}");
                    
                    if (bookingsFrom == null)
                        GetBookingsFrom(tripId, nextStation);

                    if (bookingsTo == null)
                        GetBookingsTo(tripId, nextStation);

                    if (bookingsFrom != null && bookingsFrom.Count > 0)
                    {
                        logger.LogInformation($"Notifying {bookingsFrom.Count} passengers boarding at station {nextStation.Name}");
                        await notificationService.NotifStationApproaching(bookingsFrom, lastNotificationDuration, true);
                    }

                    if (bookingsTo != null && bookingsTo.Count > 0)
                    {
                        logger.LogInformation($"Notifying {bookingsTo.Count} passengers alighting at station {nextStation.Name}");
                        await notificationService.NotifStationApproaching(bookingsTo, lastNotificationDuration, false);
                    }
                }

                if (Duration <= 1)
                {
                    logger.LogInformation($"Reached station {nextStation.Name}, moving to next");
                    StationsOrder.TryDequeue(out _);
                    bookingsFrom = null!;
                    bookingsTo = null!;
                    lastNotificationDuration = 0; 
                }
            }
        }
        private void GetBookingsTo(int tripId, Station nextStation)
        {
            bookingsTo = unitOfWork.GetRepository<Booking>()
                                   .FindAll(b => b.TripId == tripId
                                              && b.StationToId == nextStation.Id
                                              && b.Status == "Booked",
                                              new string[] { "passenger" })
                                   .Select(b => b.passenger!)
                                   .ToList();
        }

        private void GetBookingsFrom(int tripId, Station nextStation)
        {
            bookingsFrom = unitOfWork.GetRepository<Booking>()
                                     .FindAll(b => b.TripId == tripId
                                                && b.StationFromId == nextStation.Id
                                                && b.Status == "Booked",
                                                new string[] { "passenger" })
                                     .Select(b => b.passenger!)
                                     .ToList();
        }

        private void FillQueue(int tripId)
        {
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
