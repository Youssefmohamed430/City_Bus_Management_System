namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Route
    {
        public required int TripId { get; set; }
        public required int StationId { get; set; }
        public required int Order { get; set; } // Station Order in the trip
        public bool IsDeleted { get; set; } = false;
        public Station? station { get; set; }
        public Trip? trip { get; set; }
    }
}
