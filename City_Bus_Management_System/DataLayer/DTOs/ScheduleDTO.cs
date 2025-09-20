namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class ScheduleDTO
    {
        public int? SchId { get; set; }
        public DateTime Date { get; set; }
        public int? BusCode { get; set; }
        public int? BusId { get; set; }
        public string? DriverName { get; set; }
        public string? DriverId { get; set; }
    }
}
