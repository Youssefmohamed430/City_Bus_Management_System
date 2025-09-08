namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Trip
    {
        public int Id { get; set; }
        public TimeSpan Duration { get; set; } // TimeSpan.FromMinutes(90); == 01:30:00
        public required string From { get; set; }
        public required string To { get; set; }
        public List<TripSchedule>? TripSchedules { get; set; }
        public List<BookIng>? BookIngs { get; set; }
        public List<TripStations>? Tripstations { get; set; }
        public List<RevenueReport> RevenueReports { get; set; }

    }
}
