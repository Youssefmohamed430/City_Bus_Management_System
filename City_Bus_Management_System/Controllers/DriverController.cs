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
        [Authorize(Roles = "Driver")]
        public IActionResult UpdateTripStatus(string driverId, string Status)
        {
            var result = driverService.UpdateTripStatus(driverId, Status);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet]
        public IActionResult GetDrivers()
        {
            var result = driverService.GetDrivers();

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
