using System.Reflection.Metadata.Ecma335;

namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Bus
    {
        public required int BusId { get; set; }
        public required string BusCode { get; set; }
        public required string BusType { get; set; } // Silver - Gold - Platinum
        public int TotalSeats { get; set; } = 20;
        public List<TripSchedule>? TripSchedules { get; set; }
        public List<BusUsageReport> BusUsageReports { get; set; }
    }
}
