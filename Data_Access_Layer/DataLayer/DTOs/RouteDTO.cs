using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.DataLayer.DTOs
{
    public class RouteDTO
    {
        public int? TripId { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public int? StationId { get; set; }
        public string? StationName { get; set; }
        public int? Order { get; set; } // Station Order in the trip
    }
}
