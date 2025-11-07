namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Schedule
    {
        public int SchId { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public int BusId { get; set; }
        public string DriverId { get; set; }
        public int TripId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Bus? bus { get; set; }
        public Driver? driver { get; set; }
        public Trip? trip { get; set; }
    }
}
