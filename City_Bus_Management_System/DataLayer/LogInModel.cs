using System.ComponentModel.DataAnnotations;

namespace City_Bus_Management_System.DataLayer
{
    public class LogInModel
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
