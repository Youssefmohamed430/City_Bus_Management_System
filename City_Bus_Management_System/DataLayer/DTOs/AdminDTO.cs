using System.ComponentModel.DataAnnotations;

namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class AdminDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
