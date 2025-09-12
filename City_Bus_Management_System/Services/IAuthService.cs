using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Entities;

namespace City_Bus_Management_System.Services
{
    public interface IAuthService
    {
        Task<AuthModel> LogInasync(string username, string password);
        Task<AuthModel> RegisterAsPassenger(PassengerRegistertion passenger);
        Task<AuthModel> ForgotPassword(string Email);
        Task<AuthModel> ResetPassword(ResetPassModel resetPassModel);
        bool VerifyCode(string Submittedcode);
        Task<AuthModel> CreateUser();
    }
}
