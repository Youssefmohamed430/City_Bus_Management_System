using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace City_Bus_Management_System.Hubs
{
    public class TrackingHub : Hub
    {
        public IMemoryCache cache { get; set; }
        public TrackingHub(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }
        public async Task SendLocationUpdate(string busId, double latitude, double longitude)
        {
            var Location = longitude.ToString() + "," + latitude.ToString();

            cache.Set($"Bus [{busId}]", Location , TimeSpan.FromMinutes(5));

            await Clients.All.SendAsync("ReceiveLocationUpdate", busId, latitude, longitude);
        }
    }
}
