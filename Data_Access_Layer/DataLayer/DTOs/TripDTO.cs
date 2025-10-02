namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class TripDTO
    {
        public int? Id { get; set; }
        public TimeSpan Duration { get; set; } // TimeSpan.FromMinutes(90); == 01:30:00
        public string? From { get; set; }
        public string? To { get; set; }
    }
}
