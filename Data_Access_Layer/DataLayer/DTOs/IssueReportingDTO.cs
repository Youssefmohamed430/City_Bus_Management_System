

namespace Data_Access_Layer.DataLayer.DTOs
{
    public class IssueReportingDTO
    {
        public int? TripId { get; set; }
        public string? DriverId { get; set; }
        public string? DriverName { get; set; }
        public string? IssueDescription { get; set; }
    }
}
