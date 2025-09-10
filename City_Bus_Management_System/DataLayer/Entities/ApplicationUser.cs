using Microsoft.AspNetCore.Identity;

namespace City_Bus_Management_System.DataLayer.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Passenger? Passenger { get; set; }
        public Driver? Driver { get; set; }
        public List<UserNotification>? UserNotifications { get; set; }
    }
}
