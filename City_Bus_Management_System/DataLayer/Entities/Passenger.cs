namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Passenger
    {
        public required string Id { get; set; }
        public ApplicationUser? User { get; set; }

    }
}
