namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        public IRouteService routeService { get; set; }
        public RouteController(IRouteService _routeService)
        {
            routeService = _routeService;
        }
        [HttpGet("RouteForTrip/{tripId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetRouteForTrip(int tripId)
        {
            var result = routeService.GetRouteForTrip(tripId);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetRoutes()
        {
            var result = routeService.GetRoutes();

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [Authorize(Roles = "Passenger")]
        [HttpGet("TheNearestStationAtRoute/{id}/{longitude}/{latitude}")]
        public IActionResult GetTheNearestStationAtRoute(int id, double longitude, double latitude)
        {
            var result = routeService.GetTheNearestStationAtRoute(id, longitude, latitude);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddRoute([FromBody] RouteDTO route)
        {
            var result = routeService.AddRoute(route);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateRoute(int id, [FromBody] RouteDTO route)
        {
            var result = routeService.UpdateRoute(id, route);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteRoute(int id)
        {
            var result = routeService.DeleteRoute(id);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [Authorize]

        [HttpGet("CalcDistanceToDistnation/{userlng}/{userlat}/{stationlong}/{stationlat}")]
        public async Task<IActionResult> CalcDistanceToDistnation(double userlng, double userlat, double stationlong, double stationlat)
        {
            var result = await routeService.CalcDistanceToDistnation(userlng, userlat, stationlong, stationlat);

            return Ok(result);
        }
    }
}
