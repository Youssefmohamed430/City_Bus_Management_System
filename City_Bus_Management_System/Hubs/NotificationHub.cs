using Microsoft.AspNetCore.SignalR;

namespace City_Bus_Management_System.Hubs
{
    public class NotificationHub : Hub, INotificationHubService
    {
        private readonly IHubContext<NotificationHub> hubContext;

        public NotificationHub(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task SendNotificationToUser(string userId, object notification)
        {
            await hubContext.Clients.User(userId)
                .SendAsync("ReceiveNotification", notification);
        }
    }
}
