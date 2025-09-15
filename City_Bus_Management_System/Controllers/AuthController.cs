using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace City_Bus_Management_System.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly AppDbContext dbContext;
        public AuthController(IAuthService _authService,AppDbContext context)
        {
            this.authService = _authService;
            this.dbContext = context;
        }
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogInAsync(LogInDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.LogInasync(model.UserName, model.Password);

            return result.IsAuthenticated ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsPassenger(PassengerRegistertionDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.RegisterAsPassenger(model);

            return result.IsAuthenticated ? Ok(result.Message) : BadRequest(result.Message);
        }
        [HttpPost("ForgetPassword/{Email}")]
        public async Task<IActionResult> ForgetPassword(string Email)
        {
            var result = await authService.ForgotPassword(Email);

            return result.IsAuthenticated ? Ok(result.Message) : BadRequest(result.Message);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassModelDto resetPassModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.ResetPassword(resetPassModel);

            return result.IsAuthenticated ? Ok(result.Message) : BadRequest(result.Message);
        }
        [HttpPost("VerifyCode/{submittedCode}")]
        public async Task<IActionResult> VerifyCode([FromQuery] string email, string submittedCode)
        {
            var result = authService.VerifyCode(email, submittedCode);

            if (result)
            {
                var Userresult = await authService.CreateUser(email);
                return Ok(Userresult);
            }
            else
                return BadRequest(new { Message = "Invalid verification code." });
        }
        [HttpPost("DriverRequest")]
        public async Task<IActionResult> DriverRequest(DriverRequestDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.DriverRequest(model);

            return result.IsAuthenticated ? Ok(result.Message) : BadRequest(result.Message);
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(dbContext.Users.ToList());
        }
    }
}
