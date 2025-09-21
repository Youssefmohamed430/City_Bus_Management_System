using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        public ITripService tripService { get; set; }
        public TripController(ITripService _tripService)
        {
            tripService = _tripService;
        }
        [HttpGet]
        public IActionResult GetAllTrips()
        {
            var result = tripService.GetAllTrips();

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPost]
        public IActionResult AddTrip([FromBody] TripDTO trip)
        {
            if(!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var result = tripService.AddTrip(trip);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPut("{tripid}")]
        public IActionResult UpdateTrip(int tripid, [FromBody] TripDTO trip)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var result = tripService.UpdateTrip(tripid, trip);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpDelete("{tripid}")]
        public IActionResult DeleteTrip(int tripid)
        {
            var result = tripService.DeleteTrip(tripid);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
