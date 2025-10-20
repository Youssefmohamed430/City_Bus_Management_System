namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StationController : ControllerBase
    {
        public IStationService stationService { get; set; }

        public StationController(IStationService stationService)
        {
            this.stationService = stationService;
        }
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult GetStations()
        {
            var result = stationService.GetStations();  

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("{name}")]
        //[Authorize]
        public IActionResult GetStationByName(string name)
        {
            var result = stationService.GetStationByName(name);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("GetStationByArea/{area}")]
        //[Authorize]
        public IActionResult GetStationByArea(string area)
        {
            var result = stationService.GetStationsByArea(area);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("GetTheNearestStation")]
        //[Authorize]
        public IActionResult GetTheNearestStation([FromBody] LocationDTO myLocation)
        {
            var result = stationService.GetTheNearestStation(myLocation);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public IActionResult AddStation(StationDTO station)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var result = stationService.AddStation(station);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public IActionResult UpdateStation(int id , StationDTO station)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = stationService.UpdateStation(id, station);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public IActionResult DeleteStation(int id)
        {
            var result = stationService.DeleteStation(id);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
