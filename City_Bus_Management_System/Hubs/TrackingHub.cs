using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace City_Bus_Management_System.Hubs
{
    public class TrackingHub : Hub
    {
        public IMemoryCache cache { get; set; }
        public IScheduleService scheduleService { get; set; }
        public ScheduleDTO schedule = null;
        public TrackingHub(IMemoryCache memoryCache,IScheduleService _scheduleService)
        {
            cache = memoryCache;
            scheduleService = _scheduleService;
        }
        public async Task SendLocationUpdate(string driverid, double latitude, double longitude)
        {
            var Location = longitude.ToString() + "," + latitude.ToString();

            if(schedule == null)
            {
                 schedule = scheduleService.GetCurrentScheduleByDriverId(driverid);
                 if (schedule == null) return;
            }

            await Clients.All.SendAsync($"ReceiveLocationUpdate[{schedule.TripId}]", schedule.BusId,latitude, longitude);
            
            cache.Set($"Bus [{schedule.BusId}]", Location, TimeSpan.FromMinutes(5));            
        }
    }
}
