
namespace Data_Access_Layer.DataLayer.DTOs
{
    public class RouteDTO
    {
        public int? Id { get; set; }
        public int? TripId { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public int? StationId { get; set; }
        public string? StationName { get; set; }
        public int? Order { get; set; } // Station Order in the trip
    }
}
