namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Notification
    {
        public required int NotifId { get; set; }
        public required string Message { get; set; }
        public DateTime Date { get; set; }
        public List<UserNotification>? UserNotifications { get; set; }
    }
}
