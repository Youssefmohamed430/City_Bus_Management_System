using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;

namespace City_Bus_Management_System.Services
{
    public interface IAuthService
    {
        Task<AuthModel> LogInasync(string username, string password);
        Task<AuthModel> RegisterAsPassenger(PassengerRegistertionDto passenger);
        Task<AuthModel> ForgotPassword(string Email);
        Task<AuthModel> ResetPassword(ResetPassModelDto resetPassModel);
        bool VerifyCode(string email, string submittedCode);
        Task<AuthModel> CreateUser(string email);
        Task<AuthModel> DriverRequest(DriverRequestDTO model);
        Task<AuthModel> CreateAdmin(AdminDTO model);
    }
}
