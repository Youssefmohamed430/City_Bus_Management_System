namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public RevenueReport? Revenuereport { get; set; }
        public BusUsageReport? BusUsagereport { get; set; }
    }
}
