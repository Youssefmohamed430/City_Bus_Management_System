using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Services;
using Data_Access_Layer.DataLayer.DTOs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.Services
{
    public class NotificationService(IUnitOfWork unitOfWork,INotificationHubService notificationHub) : INotificationService
    {
        public Task<ResponseModel<Notification>> GetNotificationById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task NotifStationFromApproaching(List<Passenger> bookingsFrom)
        {
            foreach (var passenger in bookingsFrom)
            {
                 Notification notification = new Notification
                 {
                     Message = $"your bus is approaching your boarding station. Please be ready to board.",
                     Date = DateTime.Now
                 };

                await unitOfWork.GetRepository<Notification>().AddAsync(notification);
                await unitOfWork.SaveAsync();

                UserNotification userNotification = new UserNotification
                {
                    UserId = passenger.Id,
                    NotifId = notification.NotifId,
                };

                await unitOfWork.GetRepository<UserNotification>().AddAsync(userNotification);
                await unitOfWork.SaveAsync();

                await notificationHub.SendNotificationToUser(passenger.Id, new
                {
                    notifid = notification.NotifId,
                    msg     = notification.Message,
                    date    = notification.Date
                });
            }
        }

        public Task<ResponseModel<Notification>> NotifStationToApproaching(List<Passenger> bookingsTo)
        {
            throw new NotImplementedException();
        }
    }
}
