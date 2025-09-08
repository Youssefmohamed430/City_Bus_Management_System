namespace City_Bus_Management_System.DataLayer.Entities
{
    public class TripSchedule
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int BusId { get; set; }
        public TimeSpan DepartureTime { get; set; } // 14 PM | 2 AM
        public Trip? trip { get; set; }
        public Bus? bus { get; set; }
    }
}
