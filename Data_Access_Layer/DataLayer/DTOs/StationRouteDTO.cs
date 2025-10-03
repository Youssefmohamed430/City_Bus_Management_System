using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.DataLayer.DTOs
{
    public class StationRouteDTO : RouteDTO
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
