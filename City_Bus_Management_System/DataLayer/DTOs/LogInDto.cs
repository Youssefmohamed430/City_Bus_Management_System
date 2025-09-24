using System.ComponentModel.DataAnnotations;

namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class LogInDto
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
