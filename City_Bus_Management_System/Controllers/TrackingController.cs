using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service_Layer.Services;

namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TrackingController(TrackingService trackingService) : ControllerBase
    {
        [HttpGet("{tripId}/{buslng}/{buslat}")]
        public async Task<IActionResult> GetNextStationAtTrip(int tripId, double buslng, double buslat)
        {
            await trackingService.GetNextStationAtTrip(tripId, buslng, buslat);
            return Ok();
        }
    }
}
