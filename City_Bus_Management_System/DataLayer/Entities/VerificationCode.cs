namespace City_Bus_Management_System.DataLayer.Entities
{
    public class VerificationCode
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!; 
        public string Code { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
    }
}
