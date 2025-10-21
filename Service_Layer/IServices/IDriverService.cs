

namespace Service_Layer.IServices
{
    public interface IDriverService
    {
        ResponseModel<object> UpdateTripStatus(string driverId, string Status);

        //ResponseModel<IssueReportingDTO> IssueReporting(IssueReportingDTO issueReporting);

    }
}
