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
        private static string GetUserGroup(string userId) => $"user:{userId}";

        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            if (http != null)
            {
                var userId = http.Request.Query["userId"].FirstOrDefault();
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroup(userId));
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var http = Context.GetHttpContext();
            if (http != null)
            {
                var userId = http.Request.Query["userId"].FirstOrDefault();
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserGroup(userId));
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUser(string userId, object notification)
        {
            var groupName = GetUserGroup(userId);
            await hubContext.Clients.Group(groupName)
                .SendAsync("ReceiveNotification", notification);
        }
    }
}
