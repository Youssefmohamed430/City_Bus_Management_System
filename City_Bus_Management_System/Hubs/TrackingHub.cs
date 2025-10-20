using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace City_Bus_Management_System.Hubs
{
    public class TrackingHub : Hub
    {
        public IMemoryCache cache { get; set; }
        public IScheduleService scheduleService { get; set; }
        public IRouteService routeService { get; set; }
        public TrackingHub(IMemoryCache memoryCache,IScheduleService _scheduleService, IRouteService routeService)
        {
            cache = memoryCache;
            scheduleService = _scheduleService;
            this.routeService = routeService;
        }
        public async Task SendLocationUpdate(string driverid, double latitude, double longitude)
        {
            var Location = longitude.ToString() + "," + latitude.ToString();

            var schedule = scheduleService.GetCurrentScheduleByDriverId(driverid);

            cache.Set($"Bus [{schedule.BusId}]", Location , TimeSpan.FromMinutes(5));

            await Clients.All.SendAsync($"ReceiveLocationUpdate[{schedule.TripId}]", schedule.BusId,latitude, longitude);
        }
    }
}
