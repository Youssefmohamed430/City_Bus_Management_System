
namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class PassengerRegistertionDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "At Least three Letters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public string Name { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "In Valid Email")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone must contain numbers only")]
        public string PhoneNumber { get; set; }
        [Required]
        [MinLength(10, ErrorMessage = "At Least ten Letters")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must contain only letters and numbers")]
        public string UserName { get; set; }
        [Required]
        [MinLength(10, ErrorMessage = "At Least ten Letters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Password must contain only letters and numbers")]
        public string Password { get; set; }
    }
}
