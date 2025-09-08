namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public required int NumberOfStations { get; set; }
        public required string BusType { get; set; }
        public required double Price { get; set; } // depends on NumberOfStations and BusType
        public List<BookIng>? BookIngs { get; set; }
    }
}
