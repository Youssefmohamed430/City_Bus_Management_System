using System.Transactions;

namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public required double Balance { get; set; } = 0.0;
        public required string passengerId { get; set; }
        public Passenger? passenger { get; set; }
    }
}
