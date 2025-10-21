
namespace Service_Layer.IServices
{
    public interface IAdminService
    {
        Task<ResponseModel<DriverRequests>> AcceptDriverRequest(int RequestId);
        Task<ResponseModel<DriverRequests>> RejectDriverRequest(int RequestId);
        ResponseModel<List<DriverRequestDTO>> GetRequests();
    }
}
