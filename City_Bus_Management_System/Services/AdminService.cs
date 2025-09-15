using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Factories;
using City_Bus_Management_System.Helpers;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace City_Bus_Management_System.Services
{
    public class AdminService : IAdminService
    {
        private AppDbContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService emailService;

        public AdminService(AppDbContext _context, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            this.context = _context;
            this._userManager = userManager;
            this.emailService = emailService;
        }

        public async Task<ResponseModel<DriverRequests>> AcceptDriverRequest(int RequestId)
        {
            var request = context.DriverRequests.FirstOrDefault(x => x.Id == RequestId);

            request.Status = "Accept";

            context.Update(request);

            ApplicationUser Driveruser = new ApplicationUser
            {
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.Phone,
                UserName = request.Name + request.Id
            };

            var password = request.Name + request.Id + "23";

            var result = await _userManager.CreateAsync(Driveruser, password);

            if (!result.Succeeded)
            {
                var errors = "";

                foreach (var error in result.Errors)
                    errors += $"{error.Description}, ";

                return new ResponseModel<DriverRequests>() { IsSuccess = false, Message = errors, Result = null };
            }

            await _userManager.AddToRoleAsync(Driveruser, "Driver");

            Driveruser.EmailConfirmed = true;
            await _userManager.UpdateAsync(Driveruser);

            var driver = new Driver { SSN = request.SSN, Id = Driveruser.Id };

            context.Drivers.Add(driver);

            context.SaveChanges();

            await SendAcceptedEmail(request, Driveruser, password);

            return new ResponseModel<DriverRequests> { Message = "Request Accepted", Result = request };
        }

        private async Task SendAcceptedEmail(DriverRequests request, ApplicationUser Driveruser, string password)
        {
            var body = $@"
                        <h2 style='color:#2c3e50;'>Accepted Request to CityBus</h2>
                        <p>Your request has been <strong>accepted</strong>.</p>
                        <p>
                            <strong>User Name:</strong> {Driveruser.UserName}<br>
                            <strong>Password:</strong> {password}
                        </p>
                        <p style='color:#e74c3c;'>
                            Your password is default, please change it after your first login.
                        </p>
                        <hr>
                        <p style='font-size:12px;color:#7f8c8d;'>CityBus Team</p>
                    ";
            await emailService.SendEmailAsync(
                request.Email,
                "Accepted Request to CityBus",
                body
            );
        }

        public async Task<ResponseModel<DriverRequests>> RejectDriverRequest(int RequestId)
        {
            var request = context.DriverRequests.FirstOrDefault(x => x.Id == RequestId);

            request.Status = "Rejected";

            context.Update(request);

            context.SaveChanges();

            await emailService.SendEmailAsync(request.Email,
                 "Reject Request To CityBus"
                , "I am Sorry your request is Rejected");

            return new ResponseModel<DriverRequests> { Message = "Reject Request To CityBus", Result = null };
        }
        public ResponseModel<List<DriverRequests>> GetRequests()
        {
            var requests = context.DriverRequests.Where(_ => true);

            return new ResponseModel<List<DriverRequests>> { Message = "All Requests", Result = requests.ToList() };
        }
    }
}
