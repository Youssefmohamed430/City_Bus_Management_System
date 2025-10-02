namespace City_Bus_Management_System.DataLayer.Entities
{
    public class DriverRequests
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string SSN { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public string Status { get; set; } = "Susbend"; // Accepted - Susbend - Rejected

    }
}
