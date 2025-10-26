using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace City_Bus_Management_System.Hubs
{
    public class TrackingHub : Hub
    {
        public IMemoryCache cache { get; set; }
        public IScheduleService scheduleService { get; set; }
        public IRouteService routeService { get; set; }
        public ScheduleDTO schedule = null;
        public ITrackingService trackingService { get; set; }
        public TrackingHub(IMemoryCache memoryCache,IScheduleService _scheduleService, IRouteService routeService, ITrackingService trackingService)
        {
            cache = memoryCache;
            scheduleService = _scheduleService;
            this.routeService = routeService;
            this.trackingService = trackingService;
        }
        public async Task SendLocationUpdate(string driverid, double latitude, double longitude)
        {
            var Location = longitude.ToString() + "," + latitude.ToString();

            if(schedule == null)
                 schedule = scheduleService.GetCurrentScheduleByDriverId(driverid);

            if (schedule == null)
                return;

            trackingService?.GetNextStationAtTrip(Convert.ToInt32(schedule.TripId)!, longitude, latitude);

            cache.Set($"Bus [{schedule.BusId}]", Location, TimeSpan.FromMinutes(5));

            await Clients.All.SendAsync($"ReceiveLocationUpdate[{schedule.TripId}]", schedule.BusId,latitude, longitude);
        }
    }
}
