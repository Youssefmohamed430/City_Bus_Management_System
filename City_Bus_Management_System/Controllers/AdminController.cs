using City_Bus_Management_System.Helpers;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;


namespace City_Bus_Management_System.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
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
        [HttpPatch("AcceptRequest/{Id}")]
        public async Task<IActionResult> AcceptRequest(int Id)
        {
            var result = await adminService.AcceptDriverRequest(Id);

            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }
        [HttpPatch("RejectRequest/{RequestId}")]
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
