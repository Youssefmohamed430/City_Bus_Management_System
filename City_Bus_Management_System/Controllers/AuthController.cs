using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace City_Bus_Management_System.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        public AuthController(IAuthService _authService)
        {
            this.authService = _authService;
        }
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogInAsync(LogInModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.LogInasync(model.UserName, model.Password);

            return result.IsAuthenticated ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsPassenger(PassengerRegistertion model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.RegisterAsPassenger(model);

            return result.IsAuthenticated ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPost("ForgetPassword/{Email}")]
        public async Task<IActionResult> ForgetPassword(string Email)
        {
            var result = await authService.ForgotPassword(Email);

            return result.IsAuthenticated ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
