namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public bool IsDeleted { get; set; } = false;
        public RevenueReport? Revenuereport { get; set; }
        public BusUsageReport? BusUsagereports { get; set; }
    }
}
