namespace City_Bus_Management_System.DataLayer.Entities
{
    public class BusUsageReport
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public required DateTime ReportDate { get; set; } = DateTime.Now;
        public required double TotalTrips { get; set; }
        public required int TotalPassengers { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Report? report { get; set; }
        public Bus? bus { get; set; }
    }
}
