namespace City_Bus_Management_System.DataLayer.Entities
{
    public class DriverSchedule
    {
        public int SchId { get; set; }
        public DateTime Date { get; set; }
        public int BusId { get; set; }
        public int DriverId { get; set; }
        public Bus? bus { get; set; }
        public Driver? driver { get; set; }
    }
}
