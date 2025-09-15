namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class BusDTO
    {
        public int Id { get; set; }
        public required string BusCode { get; set; }
        public required string BusType { get; set; } // Normal - AirConditioned - Luxury
        public int TotalSeats { get; set; } = 20;
    }
}
