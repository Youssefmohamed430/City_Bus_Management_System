namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public required double Balance { get; set; } = 0.0;
        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
