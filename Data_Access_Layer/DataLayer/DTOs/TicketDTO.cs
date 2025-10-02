namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public required int MinStations { get; set; }
        public required string BusType { get; set; }
        public required double Price { get; set; } // depends on NumberOfStations and BusType
    }
}
