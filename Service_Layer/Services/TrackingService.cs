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
        private readonly IRouteService routeService;
        public TrackingService(IUnitOfWork unitOfWork, IRouteService routeService)
        {
            this.unitOfWork = unitOfWork;
            this.routeService = routeService;
        }
        public async Task GetNextStationAtTrip(int tripId,double buslng,double buslat)
        {
            var StationsOrder = new ConcurrentQueue<Station>();

            if (StationsOrder.IsEmpty)
            {
                var stations = unitOfWork.GetRepository<Route>()
                    .FindAll(r => r.TripId == tripId && !r.IsDeleted, new string[] { "station" })
                    .OrderBy(r => r.Order)
                    .Select(r => r.station)
                    .ToList();
                foreach (var station in stations)
                    StationsOrder.Enqueue(station);
            }

            if (StationsOrder.TryDequeue(out Station? nextStation))
            {
                 var coor = await routeService.CalcDistanceToDistnation(buslng, buslat, nextStation.Longitude, nextStation.Latitude);

                if (coor.Duration == 15 || coor.Duration == 10 || coor.Duration == 5)
                {
                    // Notify passengers with [ FromStation == NextStation ].
                }
            }
        }
    }
}
