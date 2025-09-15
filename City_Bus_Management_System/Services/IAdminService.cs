using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Entities;

namespace City_Bus_Management_System.Services
{
    public interface IAdminService
    {
        Task<ResponseModel<DriverRequests>> AcceptDriverRequest(int RequestId);
        Task<ResponseModel<DriverRequests>> RejectDriverRequest(int RequestId);
    }
}
