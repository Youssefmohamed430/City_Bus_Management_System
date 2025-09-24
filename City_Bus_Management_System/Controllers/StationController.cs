using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetStations()
        {
            var result = stationService.GetStations();  

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("{name}")]
        public IActionResult GetStationByName(string name)
        {
            var result = stationService.GetStationByName(name);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("GetStationByArea/{area}")]
        public IActionResult GetStationByArea(string area)
        {
            var result = stationService.GetStationByArea(area);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("GetTheNearestStation/{Longitude}/{Latitude}")]
        public IActionResult GetTheNearestStation(decimal Longitude,decimal Latitude)
        {
            var result = stationService.GetTheNearestStation(Longitude, Latitude);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost]
        public IActionResult AddStation(StationDTO station)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var result = stationService.AddStation(station);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateStation(int id , StationDTO station)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = stationService.UpdateStation(id, station);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteStation(int id)
        {
            var result = stationService.DeleteStation(id);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
