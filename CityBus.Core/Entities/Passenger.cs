namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Passenger
    {
        public required string Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ApplicationUser? User { get; set; }
        public List<Booking>? BookIngs { get; set; }
        public Wallet? wallet { get; set; }
    }
}
