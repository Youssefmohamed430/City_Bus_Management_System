using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Factories;
using City_Bus_Management_System.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace City_Bus_Management_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly JWTService _jwtservice;

        public AuthService(UserManager<ApplicationUser> userManager, AppDbContext context, JWTService jwtservice)
        {
            _userManager = userManager;
            _context = context;
            _jwtservice = jwtservice;
        }

        public async Task<AuthModel> LogInasync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || !await _userManager.CheckPasswordAsync(user ,password))
                return new AuthModel { Message = "Invalid username or password", IsAuthenticated = false };

            var JWTSecurityToken = await _jwtservice.CreateJwtToken(user);


            return new AuthModelFactory()
                .CreateAuthModel(user.Id, user.UserName, user.Email, JWTSecurityToken.ValidTo,
                JWTSecurityToken.Claims.Where(x => x.Type == "roles").Select(x => x.Value).ToList()
                , new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken));
        
        }

        public async Task<AuthModel> RegisterAsPassenger(PassengerRegistertion model)
        {
            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel() { Message = "User Name Is Already Registerd" };

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel() { Message = "Email Is Already Registerd" };

            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.Username,
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

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

            return new AuthModelFactory()
                .CreateAuthModel(user.Id, model.Username, model.Email, JWTSecurityToken.ValidTo, new List<string> { "Passenger" }, new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken));
        
        }
    }
}
