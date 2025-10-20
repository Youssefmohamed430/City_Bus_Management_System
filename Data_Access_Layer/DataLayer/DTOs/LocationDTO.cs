

namespace Data_Access_Layer.DataLayer.DTOs
{
    public class LocationDTO
    {
        [Required]
        public required string Area { get; set; }
        [Required]
        public double Longitude { get; set; } // 30.089952
        [Required]
        public double Latitude { get; set; } // 31.204495

    }
}
