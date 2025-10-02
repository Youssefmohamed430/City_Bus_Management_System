using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Factories;
using City_Bus_Management_System.Helpers;
using Core_Layer;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Service_Layer.IServices;
using System.IdentityModel.Tokens.Jwt;

namespace City_Bus_Management_System.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService emailService;
        private readonly ILogger<AdminService> logger;
        private readonly IUnitOfWork unitOfWork;

        public AdminService(UserManager<ApplicationUser> userManager, IEmailService emailService, ILogger<AdminService> logger)
        {
            this._userManager = userManager;
            this.emailService = emailService;
            this.logger = logger;
        }

        public async Task<ResponseModel<DriverRequests>> AcceptDriverRequest(int RequestId)
        {
            var request = unitOfWork.DriverReqs.Find(x => x.Id == RequestId);

            if(request == null)
                return new ResponseModel<DriverRequests>() { IsSuccess = false, Message = "Request Not Found", Result = null! };

            request.Status = "Accept";

            await unitOfWork.DriverReqs.UpdateAsync(request);

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

                logger.LogError("Error Creating Driver User: {Errors}", errors);

                return new ResponseModel<DriverRequests>() { IsSuccess = false, Message = errors, Result = null };
            }

            await _userManager.AddToRoleAsync(Driveruser, "Driver");

            Driveruser.EmailConfirmed = true;
            await _userManager.UpdateAsync(Driveruser);

            var driver = new Driver { SSN = request.SSN, Id = Driveruser.Id };

            await unitOfWork.Drivers.AddAsync(driver);
            await unitOfWork.SaveAsync();

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
            var request = unitOfWork.DriverReqs.Find(x => x.Id == RequestId);

            if (request == null)
                return new ResponseModel<DriverRequests>() { IsSuccess = false, Message = "Request Not Found", Result = null };

            request.Status = "Rejected";

            await unitOfWork.DriverReqs.UpdateAsync(request);

            await unitOfWork.SaveAsync();

            var body = $@"<p>Your request has been <strong>accepted</strong>.</p> 
                        <hr>
                       <p style='font-size:12px;color:#7f8c8d;'>CityBus Team</p>";

            await emailService.SendEmailAsync(request.Email,"Reject Request To CityBus",body);

            return new ResponseModel<DriverRequests> { Message = "Reject Request To CityBus", Result = null };
        }
        public ResponseModel<List<DriverRequestDTO>> GetRequests()
        {
            var requests = unitOfWork.DriverReqs.FindAll<DriverRequestDTO>(_ => true);

            return new ResponseModel<List<DriverRequestDTO>> { Message = "All Requests", Result = requests.ToList() };
        }
    }
}
