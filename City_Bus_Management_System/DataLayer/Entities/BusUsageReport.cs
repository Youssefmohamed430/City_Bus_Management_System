namespace City_Bus_Management_System.DataLayer.Entities
{
    public class BusUsageReport
    {
        public int Id { get; set; }
        public required DateTime ReportDate { get; set; } = DateTime.Now;
        public required double TotalTrips { get; set; }
        public required int TotalPassengers { get; set; }
        public Report? report { get; set; }
        public Bus? bus { get; set; }
    }
}
