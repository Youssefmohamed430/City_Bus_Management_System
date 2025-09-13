using System.ComponentModel.DataAnnotations;

namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class DriverRequestDTO
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string SSN { get; set; }
        [Required]
        public required string Phone { get; set; }
        [Required]
        public required string Email { get; set; }
    }
}
