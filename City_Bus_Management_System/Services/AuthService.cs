using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Factories;
using City_Bus_Management_System.Helpers;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
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
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthService> logger;


        public AuthService(UserManager<ApplicationUser> userManager, AppDbContext context, JWTService jwtservice, SignInManager<ApplicationUser> signInManager, IEmailService emailService, IConfiguration configuration, IMemoryCache cache, ILogger<AuthService> _logger)
        {
            _userManager = userManager;
            _context = context;
            _jwtservice = jwtservice;
            _signInManager = signInManager;
            this.emailService = emailService;
            _configuration = configuration;
            _cache = cache;
            this.logger = _logger;
        }
        public async Task<AuthModel> LogInasync(string username, string password)
        {
            logger.LogInformation("Login attempt for user {UserName}", username);

            var result = await _signInManager.PasswordSignInAsync(
                             username,
                             password,
                             isPersistent: false,
                             lockoutOnFailure: true);
            
            if (result.Succeeded)
            {   
                var user = await _userManager.FindByNameAsync(username);
                var token = await _jwtservice.CreateJwtToken(user);

                if (user.RefreshTokens.Any(t => t.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                    
                    return new AuthModelFactory()
                    .CreateAuthModel(user.Id, user.UserName, user.Email, token.ValidTo,
                        token.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList(),
                        new JwtSecurityTokenHandler().WriteToken(token),activeRefreshToken.Token,activeRefreshToken.ExpiresOn);
                }
                else
                {
                    var refreshToken = GenerateRefreshToken();

                    user.RefreshTokens.Add(refreshToken);
                    await _userManager.UpdateAsync(user);

                    return new AuthModelFactory()
                    .CreateAuthModel(user.Id, user.UserName, user.Email, token.ValidTo,
                        token.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList(),
                        new JwtSecurityTokenHandler().WriteToken(token), refreshToken.Token, refreshToken.ExpiresOn); 
                }         
            }
            else if (result.IsLockedOut)
            {
                logger.LogWarning("User {UserName} account locked out", username);

                return new AuthModel { Message = "Account locked due to multiple invalid attempts.", IsAuthenticated = false };
            }
            else
            {
                logger.LogWarning("Invalid login for {UserName}", username);
                
                return new AuthModel { Message = "Invalid username or password", IsAuthenticated = false };
            }
        }
        public async Task<AuthModel> RegisterAsPassenger(PassengerRegistertionDto passengermodel)
        {
            if (await _userManager.FindByNameAsync(passengermodel.UserName) is not null)
                return new AuthModel() { Message = "User Name Is Already Registerd" };

            if (await _userManager.FindByEmailAsync(passengermodel.Email) is not null)
                return new AuthModel() { Message = "Email Is Already Registerd" };

            await VerificationAccount(passengermodel.Email);

            var cacheKey = $"passenger:{passengermodel.Email}";
            _cache.Set(cacheKey,passengermodel, TimeSpan.FromMinutes(10));

            return new AuthModel { Message = "Verification code sent to email.", IsAuthenticated = true };
        }
        public async Task<AuthModel> CreateUser(string email)
        {
            if(!_cache.TryGetValue($"passenger:{email}", out PassengerRegistertionDto? model))
                return new AuthModel() { Message = "Verification code expired or invalid." };

            var user = model.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = "";

                foreach (var error in result.Errors)
                    errors += $"{error.Description}, ";

                logger.LogError("Failed to create user {Email}. Errors: {Errors}", email, errors);

                return new AuthModel() { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "Passenger");

            var JWTSecurityToken = await _jwtservice.CreateJwtToken(user);

            user.EmailConfirmed = true;
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            logger.LogInformation("Passenger account created successfully for {Email}", email);

            return new AuthModelFactory()
                .CreateAuthModel(user.Id, model.UserName, model.Email, JWTSecurityToken.ValidTo, new List<string> { "Passenger" }, new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken),refreshToken.Token,refreshToken.ExpiresOn,"Code Verfied successfully!");
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
        public async Task<AuthModel> ResetPassword(ResetPassModelDto resetPassModel)
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
        private async Task<string?> VerificationAccount(string email)
        {
            var verificationCode = Random.Shared.Next(100000, 999999).ToString();

            try
            {
                await emailService.SendEmailAsync(
                    email,
                    "Verification Account",
                    $"{verificationCode} is your verification code for your security."
                );

                _cache.Set(email, verificationCode, TimeSpan.FromMinutes(10));

                return verificationCode;
            }
            catch
            {
                return null;
            }
        }
        public bool VerifyCode(string email, string submittedCode)
        {
            if (_cache.TryGetValue(email, out string? code))
            {
                return code == submittedCode;
            }
            return false;
        }
        public async Task<AuthModel> DriverRequest(DriverRequestDTO model)
        {
            //var driverR = new DriverRequests
            //{
            //    Name = model.Name,
            //    Email = model.Email,
            //    Phone = model.Phone,
            //    SSN = model.SSN,
            //};

            var driverR = model.Adapt<DriverRequests>();

            _context.DriverRequests.Add(driverR);

            await _context.SaveChangesAsync();

            return new AuthModel { IsAuthenticated = true , Message = "Driver request submitted successfully." };
        }
        public async Task<AuthModel> CreateAdmin(AdminDTO model)
        {
            var user = model.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = "";

                foreach (var error in result.Errors)
                    errors += $"{error.Description}, ";

                return new AuthModel() { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "Admin");

            var JWTSecurityToken = await _jwtservice.CreateJwtToken(user);

            user.EmailConfirmed = true;
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthModelFactory()
                .CreateAuthModel(user.Id, model.UserName, model.Email, JWTSecurityToken.ValidTo, new List<string> { "Admin" }, new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken),refreshToken.Token,refreshToken.ExpiresOn);
        }
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            RandomNumberGenerator.Fill(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }
        public async Task<AuthModel> RefreshToken(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if(user == null)
                return new AuthModel { Message = "Invalid token." };

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return new AuthModel { Message = "InActive token." };

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var JWTSecurityToken = await _jwtservice.CreateJwtToken(user);

            return new AuthModelFactory()
                .CreateAuthModel(user.Id, user.UserName, user.Email, JWTSecurityToken.ValidTo,
                    JWTSecurityToken.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList(),
                    new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken), newRefreshToken.Token, newRefreshToken.ExpiresOn);
        }
        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true; 
        }
    }
}
