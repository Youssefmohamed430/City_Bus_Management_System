namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class ScheduleDTO
    {
        public int? SchId { get; set; }
        public TimeSpan DepartureTime { get; set; } // 14 PM | 2 AM
        public string? BusCode { get; set; }
        public int? BusId { get; set; }
        public string? DriverName { get; set; }
        public string? DriverId { get; set; }
        public int? TripId { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
    }
}
