using Core_Layer.Entities;

namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Driver
    {
        public required string Id { get; set; }
        public string? Image { get; set; }
        public required string SSN { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ApplicationUser? User { get; set; }
        public DriverStatistics? DriverStatistics { get; set; }
        public List<Schedule>? Schedules { get; set; }
    }
}
