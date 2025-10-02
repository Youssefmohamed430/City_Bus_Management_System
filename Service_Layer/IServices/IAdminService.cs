using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;

namespace Service_Layer.IServices
{
    public interface IAdminService
    {
        Task<ResponseModel<DriverRequests>> AcceptDriverRequest(int RequestId);
        Task<ResponseModel<DriverRequests>> RejectDriverRequest(int RequestId);
        ResponseModel<List<DriverRequestDTO>> GetRequests();
    }
}
