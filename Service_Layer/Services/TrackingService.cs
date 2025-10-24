using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.Services
{
    public class TrackingService
    {
        private readonly IUnitOfWork unitOfWork;
        public ConcurrentQueue<Station> StationsOrder = new ConcurrentQueue<Station>();
        public List<Passenger> bookingsFrom;
        public List<Passenger> bookingsTo;
        private readonly IRouteService routeService;
        private INotificationService notificationService;
        public TrackingService(IUnitOfWork unitOfWork, IRouteService routeService, INotificationService notificationService)
        {
            this.unitOfWork = unitOfWork;
            this.routeService = routeService;
            this.notificationService = notificationService;
        }
        public async Task GetNextStationAtTrip(int tripId,double buslng,double buslat)
        {
            if (StationsOrder.IsEmpty)
                FillQueue(tripId);

            if (StationsOrder.TryPeek(out Station? nextStation))
            {
                 var coor = await routeService.CalcDistanceToDistnation(buslng, buslat, nextStation.Longitude, nextStation.Latitude);
                 int Duration = (int)coor.Duration;

                if (Duration == 15 || Duration == 10 || Duration == 5)
                {
                    if(bookingsFrom == null)
                        GetBookingsFrom(tripId, nextStation);

                    if(bookingsTo == null)
                        GetBookingsTo(tripId, nextStation);

                    // Notify passengers with [ FromStation == NextStation ].
                    if (bookingsFrom != null && bookingsFrom.Count > 0)
                        await notificationService.NotifstationsproximityFrom(bookingsFrom);

                    // Notify passengers with [ ToStation == NextStation ].
                    if (bookingsTo != null && bookingsTo.Count > 0)
                        await notificationService.NotifstationsproximityTo(bookingsTo);
                }

                if (Duration <= 1)
                {
                    StationsOrder.TryDequeue(out _);
                    bookingsFrom = null!;
                    bookingsTo = null!;
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
