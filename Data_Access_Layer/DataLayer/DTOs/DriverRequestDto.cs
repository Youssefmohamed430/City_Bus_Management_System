
namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class DriverRequestDTO
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "At Least three Letters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public required string Name { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "SSN must contain numbers only")]
        [MinLength(14, ErrorMessage = "At Least 14 digt")]
        public required string SSN { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone must contain numbers only")]
        public required string Phone { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "In Valid Email")]
        public required string Email { get; set; }
        public string? Status { get; set; }
    }
}
