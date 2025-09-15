using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Factories;
using City_Bus_Management_System.Helpers;
using City_Bus_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;


namespace City_Bus_Management_System.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private AppDbContext context;
        private IAdminService adminService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IAdminService adminService, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.adminService = adminService;
            this.context = context;
            _userManager = userManager;
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
        [HttpGet]
        public IActionResult GetRequests()
        {
            var result = adminService.GetRequests();
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        
    }
}
