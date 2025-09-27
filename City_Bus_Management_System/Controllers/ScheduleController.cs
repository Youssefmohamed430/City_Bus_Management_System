using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetSchedules()
        {
            var result = _scheduleService.GetSchedules();

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{Id}")]
        [Authorize(Roles = "Driver")]
        public IActionResult GetSchedulesByDriverId(string Id)
        {
            var result = _scheduleService.GetSchedulesByDriverId(Id);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("GetSchByName/{Name}")]
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
    }
}
