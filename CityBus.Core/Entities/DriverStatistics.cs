using City_Bus_Management_System.DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities
{
    public class DriverStatistics
    {
        public int Id { get; set; }
        public string DriverId { get; set; }
        public int TotalTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledTrips { get; set; }
        public DateTime UpdateTime { get; set; } 
        public Driver driver { get; set; }
    }
}
