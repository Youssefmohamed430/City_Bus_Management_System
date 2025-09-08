namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Driver
    {
        public required string Id { get; set; }
        public string? Image { get; set; }
        public required string SSN { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
