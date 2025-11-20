namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Trip
    {
        public int Id { get; set; }
        public TimeSpan Duration { get; set; } // TimeSpan.FromMinutes(90); == 01:30:00
        public required string From { get; set; }
        public required string To { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<Schedule>? Schedules { get; set; }
        public List<Booking>? Bookings { get; set; }
        public List<Route>? Routes { get; set; }
        //public List<RevenueReport>? RevenueReports { get; set; }

    }
}
