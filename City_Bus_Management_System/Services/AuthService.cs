using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Factories;
using City_Bus_Management_System.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace City_Bus_Management_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly JWTService _jwtservice;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService emailService;
        private string email;
        private readonly IConfiguration _configuration;
        public string code;
        public PassengerRegistertion model;

        public AuthService(UserManager<ApplicationUser> userManager, AppDbContext context, JWTService jwtservice, SignInManager<ApplicationUser> signInManager, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _jwtservice = jwtservice;
            _signInManager = signInManager;
            this.emailService = emailService;
            _configuration = configuration;
        }
        public async Task<AuthModel> LogInasync(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(
                             username,
                             password,
                             isPersistent: false,
                             lockoutOnFailure: true); 

            if (result.Succeeded)
            {   
                var user = await _userManager.FindByNameAsync(username);
                var token = await _jwtservice.CreateJwtToken(user);

                return new AuthModelFactory()
                    .CreateAuthModel(user.Id, user.UserName, user.Email, token.ValidTo,
                        token.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList(),
                        new JwtSecurityTokenHandler().WriteToken(token));
            }
            else if (result.IsLockedOut)
            {
                return new AuthModel { Message = "Account locked due to multiple invalid attempts.", IsAuthenticated = false };
            }
            else
            {
                return new AuthModel { Message = "Invalid username or password", IsAuthenticated = false };
            }
        }
        public async Task<AuthModel> RegisterAsPassenger(PassengerRegistertion passengermodel)
        {
            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel() { Message = "User Name Is Already Registerd" };

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel() { Message = "Email Is Already Registerd" };
            
            code = VerficationAccount(email).ToString()!;

            model = passengermodel;

            return new AuthModel { Message = "Verification code sent to email.", IsAuthenticated = true };
        }

        public async Task<AuthModel> CreateUser()
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.Username,
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = "";

                foreach (var error in result.Errors)
                    errors += $"{error.Description}, ";

                return new AuthModel() { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "Passenger");

            var JWTSecurityToken = await _jwtservice.CreateJwtToken(user);

            return new AuthModelFactory()
                .CreateAuthModel(user.Id, model.Username, model.Email, JWTSecurityToken.ValidTo, new List<string> { "Passenger" }, new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken),"Code Verfied successfully!");
        }

        public async Task<AuthModel> ForgotPassword(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
                return new AuthModel { Message = "If this email address is registered with us, password reset instructions will be sent to it." };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var head = @"https://localhost:44382/";

            var ResetLink = $"{head}/Auth/ResetPassword?email={Email}&token={Uri.EscapeDataString(token)}";

            await emailService.SendEmailAsync(user.Email, "Reset Password", $"Click here to reset: {ResetLink}");

            return new AuthModel { Message = "Reset password link has been sent.", IsAuthenticated = true};
        }
        public async Task<AuthModel> ResetPassword(ResetPassModel resetPassModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPassModel.Email);

            if (user == null)
                return new AuthModel { Message = "Invalid request."};

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPassModel.token));

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPassModel.NewPassword);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthModel { Message = errors };
            }

            return new AuthModel { IsAuthenticated = true, Message = "Password reset successfully." };
        }
        private async Task<string> VerficationAccount(string email)
        {
            var verificationCode = Random.Shared.Next(100000, 999999).ToString();
            try
            {
                await emailService.SendEmailAsync(email, "Verfication Account", $"{verificationCode} is your verfification code for your security.");
                return verificationCode;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public bool VerifyCode(string Submittedcode)
            => code == Submittedcode;
    }
}
