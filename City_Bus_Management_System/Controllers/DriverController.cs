namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        public IDriverService driverService { get; set; }
        public DriverController(IDriverService _driverService)
        {
            driverService = _driverService;
        }
        [HttpPut("{driverId}/{Status}")]
        public IActionResult UpdateTripStatus(string driverId, string Status)
        {
            var result = driverService.UpdateTripStatus(driverId, Status);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
