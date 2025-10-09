namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class TicketDTO
    {
        public int? Id { get; set; }
        public int? MinStations { get; set; }
        public string? BusType { get; set; }
        public double? Price { get; set; } // depends on NumberOfStations and BusType
    }
}
