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
    public class AdminService
        (UserManager<ApplicationUser> _userManager, IEmailService emailService, ILogger<AdminService> logger, IUnitOfWork unitOfWork) : IAdminService
    {
        public async Task<ResponseModel<DriverRequests>> AcceptDriverRequest(int RequestId)
        {
            var request = unitOfWork.GetRepository<DriverRequests>().Find(x => x.Id == RequestId);

            if(request == null)
                return new ResponseModel<DriverRequests>() { IsSuccess = false, Message = "Request Not Found", Result = null! };

            request.Status = "Accept";

            await unitOfWork.GetRepository<DriverRequests>().UpdateAsync(request);

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

            await unitOfWork.GetRepository<Driver>().AddAsync(driver);
            await unitOfWork.SaveAsync();

            await SendAcceptedEmail(request, Driveruser, password);

            return new ResponseModel<DriverRequests> { Message = "Request Accepted", Result = request };
        }
        private async Task SendAcceptedEmail(DriverRequests request, ApplicationUser Driveruser, string password)
        {
            var htmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AcceptDriverRequestEmailTemplate.html");
            var htmlBody = File.ReadAllText(htmlPath);

            htmlBody = htmlBody.Replace("{{DriverUserName}}", Driveruser.UserName);
            htmlBody = htmlBody.Replace("{{password}}", password);

            await emailService.SendEmailAsync(
                request.Email,
                "Accepted Request to CityBus",
                htmlBody
            );
        }
        public async Task<ResponseModel<DriverRequests>> RejectDriverRequest(int RequestId)
        {
            var request = unitOfWork.GetRepository<DriverRequests>().Find(x => x.Id == RequestId);

            if (request == null)
                return new ResponseModel<DriverRequests>() { IsSuccess = false, Message = "Request Not Found", Result = null };

            request.Status = "Rejected";

            await unitOfWork.GetRepository<DriverRequests>().UpdateAsync(request);

            await unitOfWork.SaveAsync();

            var htmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "RejectedRequestEmailTemplate.html");
            var htmlBody = File.ReadAllText(htmlPath);

            await emailService.SendEmailAsync(request.Email,"Reject Request To Waselny",htmlBody);

            return new ResponseModel<DriverRequests> { Message = "Reject Request To Waselny", Result = null! };
        }
        public ResponseModel<List<DriverRequestDTO>> GetRequests()
        {
            var requests = unitOfWork.GetRepository<DriverRequests>().FindAll<DriverRequestDTO>(_ => true);

            return new ResponseModel<List<DriverRequestDTO>> { Message = "All Requests", Result = requests.ToList() };
        }
    }
}
