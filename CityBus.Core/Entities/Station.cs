

namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Station
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Area { get; set; }
        public required string Location { get; set; }
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<Route>? Routes { get; set; }
        public List<Booking> FromStationsBookings { get; set; }
        public List<Booking> ToStationsBookings { get; set; }
    }
}
