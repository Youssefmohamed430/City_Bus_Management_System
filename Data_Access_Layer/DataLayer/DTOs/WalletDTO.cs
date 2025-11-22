
namespace Data_Access_Layer.DataLayer.DTOs
{
    public class WalletDTO
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? email { get; set; }
        public double Balance { get; set; } = 0.0;
        public required string passengerId { get; set; }
    }
}
