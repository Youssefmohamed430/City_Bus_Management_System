using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace City_Bus_Management_System.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private AppDbContext context;
        private IAdminService adminService;

        public AdminController(IAdminService adminService, AppDbContext context)
        {
            this.adminService = adminService;
            this.context = context;
        }
        [HttpPost("AcceptRequest/{Id}")]
        public async Task<IActionResult> AcceptRequest(int Id)
        {
            var result = await adminService.AcceptDriverRequest(Id);

            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }
        [HttpPost("RejectRequest/{RequestId}")]
        public async Task<IActionResult> RejectRequest(int RequestId)
        {
            var result = await adminService.RejectDriverRequest(RequestId);

            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
