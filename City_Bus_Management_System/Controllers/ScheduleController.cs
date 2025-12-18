using City_Bus_Management_System.Attributes;

namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        public IScheduleService _scheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Cacheable("schedules")]
        public IActionResult GetSchedules()
        {
            var result = _scheduleService.GetSchedules();

            return result != null ? Ok(result) : BadRequest();
        }

        [HttpGet("ByDriverId/{Id}")]
        [Authorize(Roles = "Driver")]
        public IActionResult GetSchedulesByDriverId(string Id)
        {
            var result = _scheduleService.GetSchedulesByDriverId(Id);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("ByDriverName/{Name}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetSchedulesByDriverName(string Name)
        {
            var result = _scheduleService.GetSchedulesByDriverName(Name);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddSchedule([FromBody] ScheduleDTO Schedule)
        {
            if(!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var result = _scheduleService.AddSchedule(Schedule);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPut("{SchId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateDriverSchedule(int SchId, [FromBody] ScheduleDTO newSchedule)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var result = _scheduleService.UpdateDriverSchedule(SchId, newSchedule);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("{SchId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult RemoveDriverSchedule(int SchId)
        {
            var result = _scheduleService.RemoveDriverSchedule(SchId);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("ByTripId/{tripid}")]
        [Authorize]
        public IActionResult GetSchedulesByTripId(int tripid)
        {
            var result = _scheduleService.GetSchedulesByTripId(tripid);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("CurrentByDriverId/{Id}")]
        [Authorize(Roles = "Driver")]
        public IActionResult GetCurrentScheduleByDriverId(string Id)
        {
            var result = _scheduleService.GetCurrentScheduleByDriverId(Id);

            return result != null ? Ok(result) : BadRequest("No Current Schedule Found");
        }
    }
}
