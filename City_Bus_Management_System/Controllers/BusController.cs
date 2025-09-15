using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace City_Bus_Management_System.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class BusController : Controller
    {
        private readonly IBusService busService;
        public BusController(IBusService _busservice)
        {
            this.busService = _busservice;
        }
        [HttpPost]
        public async Task<IActionResult> AddBus([FromBody] BusDTO bus)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await busService.AddBus(bus);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet]
        public IActionResult GetBuses()
        {
            var result = busService.GetBuses();

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateBus([FromBody]BusDTO EditedBus,int Id)
        {
            var result = await busService.UpdateBus(EditedBus,Id);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteBus(int Id)
        {
            var result = await busService.DeleteBus(Id);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("GetBusByCode/{Code}")]
        public IActionResult GetBusByCode(string Code)
        {
            var result = busService.GetBusByCode(Code);

            return result.IsSuccess ? Ok(result) : NotFound(result.Message);
        }
        [HttpGet("GetBusesByType/{Type}")]
        public IActionResult GetBusesByType(string Type)
        {
            var result = busService.GetBusByType(Type);

            return result.IsSuccess ? Ok(result) : NotFound(result.Message);
        }
    }
}
