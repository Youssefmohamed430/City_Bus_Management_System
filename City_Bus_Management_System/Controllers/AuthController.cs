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
        public AuthController(IAuthService _authService, AppDbContext context)
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

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return result.IsAuthenticated ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsPassenger(PassengerRegistertionDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.RegisterAsPassenger(model);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

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
        [HttpPost("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin(AdminDTO model)
        {
            var result = await authService.CreateAdmin(model);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return result.IsAuthenticated ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { Message = "No refresh token provided." });

            var result = await authService.RefreshToken(refreshToken);

            if(!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return result.IsAuthenticated ? Ok(result) : Unauthorized(result.Message);

        }
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await authService.RevokeTokenAsync(token);

            if (!result)
                return BadRequest("Token is invalid!");

            return Ok();
        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
