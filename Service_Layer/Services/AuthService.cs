using City_Bus_Management_System.DataLayer.Data;
using Data_Access_Layer.Helpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace City_Bus_Management_System.Services
{
    public class AuthService(UserManager<ApplicationUser> _userManager, AppDbContext _context,
            JWTService _jwtservice, SignInManager<ApplicationUser> _signInManager,
            IEmailService emailService, IConfiguration _configuration,
            IMemoryCache _cache, ILogger<AuthService> logger, IWalletService walletService, IUnitOfWork unitOfWork) : IAuthService
    {
        public async Task<AuthModel> LogInasync(string username, string password)
        {
            logger.LogInformation("Login attempt for user {UserName}", username);

            var result = await _signInManager.PasswordSignInAsync(
                  username,password,isPersistent: false,lockoutOnFailure: true);
            
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(username);

                var token = await _jwtservice.CreateJwtToken(user);

                var refreshtoken =  await HandleRefreshToken(user, token);

                return new AuthModelFactory()
                    .CreateAuthModel(user.Id, user.UserName, user.Email, token.ValidTo,
                        token.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList(),
                        new JwtSecurityTokenHandler().WriteToken(token), refreshtoken.Token,EgyptTimeHelper.ConvertFromUtc(refreshtoken.ExpiresOn));
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

        private async Task<RefreshToken> HandleRefreshToken(ApplicationUser user, JwtSecurityToken token)
        {
            RefreshToken refreshtoken = null;

            if (user.RefreshTokens.Any(t => t.IsActive))
                refreshtoken = user.RefreshTokens.FirstOrDefault(t => t.IsActive)!;
            else
            {
                refreshtoken = GenerateRefreshToken();
                user.RefreshTokens.Add(refreshtoken);
                await _userManager.UpdateAsync(user);
            }

            return refreshtoken;
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
            try
            {
                unitOfWork.BeginTransaction();

                if (!_cache.TryGetValue($"passenger:{email}", out PassengerRegistertionDto? model))
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

                var passenger = user.Adapt<Passenger>();

                await unitOfWork.GetRepository<Passenger>().AddAsync(passenger);

                await unitOfWork.SaveAsync();

                walletService.CreateWallet(new WalletDTO { passengerId = user.Id });

                var JWTSecurityToken = await _jwtservice.CreateJwtToken(user);

                user.EmailConfirmed = true;

                var refreshToken = await HandleRefreshToken(user, JWTSecurityToken);

                logger.LogInformation("Passenger account created successfully for {Email}", email);

                unitOfWork.Commit();

                return new AuthModelFactory()
                    .CreateAuthModel(user.Id, model.UserName, model.Email, JWTSecurityToken.ValidTo, new List<string> { "Passenger" }, new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken), refreshToken.Token, refreshToken.ExpiresOn, "Code Verfied successfully!");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                logger.LogError(ex, "Error creating user for {Email}", email);
                return new AuthModel { Message = "An error occurred while creating the user." };
            }
        }
        public async Task<AuthModel> ForgotPassword(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
                return new AuthModel { Message = "If this email address is registered with us, password reset instructions will be sent to it." };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            string htmlBody = HandleForgotEmailBody(Email, token);

            await emailService.SendEmailAsync(user.Email, "Reset Password", htmlBody);

            return new AuthModel { Message = "Reset password link has been sent.", IsAuthenticated = true };
        }

        private string HandleForgotEmailBody(string Email, string token)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Waselny.jpg");
            var imageBytes = File.ReadAllBytes(imagePath);
            var base64Image = Convert.ToBase64String(imageBytes);
            var imageDataUrl = $"data:image/jpeg;base64,{base64Image}";


            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:44382";
            var resetLink = $"{baseUrl}/Auth/ResetPassword?email={Uri.EscapeDataString(Email)}&token={Uri.EscapeDataString(token)}";

            var htmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FogotPasswordEmailTemplate.html");
            var htmlBody = File.ReadAllText(htmlPath);
            htmlBody = htmlBody.Replace("{{LogoImage}}", imageDataUrl);
            htmlBody = htmlBody.Replace("{{ResetLink}}", resetLink);
            return htmlBody;
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

            var htmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" , "VerificationCodeEmail.html");
            var htmlBody = File.ReadAllText(htmlPath);

            htmlBody = htmlBody.Replace("{{CODE}}", verificationCode);
            htmlBody = htmlBody.Replace("{{DATE}}", DateTime.Now.ToString("yyyy"));

            try
            {
                await emailService.SendEmailAsync(
                    email,
                    "Verification Account",
                    htmlBody
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
            var refreshToken = await HandleRefreshToken(user, JWTSecurityToken);

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
                ExpiresOn = EgyptTimeHelper.ConvertToUtc(EgyptTimeHelper.Now).AddDays(10),
                CreatedOn = EgyptTimeHelper.ConvertToUtc(EgyptTimeHelper.Now)
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

            refreshToken.RevokedOn = EgyptTimeHelper.ConvertToUtc(EgyptTimeHelper.Now);

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
