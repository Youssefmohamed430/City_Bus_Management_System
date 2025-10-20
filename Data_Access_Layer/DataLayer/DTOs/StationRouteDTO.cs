
namespace Data_Access_Layer.DataLayer.DTOs
{
    public class StationRouteDTO : RouteDTO
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
    }
}
