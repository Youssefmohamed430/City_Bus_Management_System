namespace City_Bus_Management_System.DataLayer.Entities
{
    public class RevenueReport
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public required DateTime ReportDate { get; set; } = DateTime.Now;
        public required double TotalRevenue { get; set; }
        public int NumberOfTickets { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Report? report { get; set; }
        public Trip? trip { get; set; }
    }
}
