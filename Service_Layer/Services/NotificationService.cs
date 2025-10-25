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

        public async Task NotifStationApproaching(List<Passenger> bookingsFrom,int dur,bool isfrom)
        {
            Notification notification = new Notification
            {
                Message =  isfrom ?
                $"your bus is approaching your boarding station remaining {dur} minutes to approach. Please be ready to board." :
                $"your bus is approaching your arrival station remaining {dur} minutes to arrive. Please be ready to go down.",

                Date = DateTime.UtcNow
            };

            await unitOfWork.GetRepository<Notification>().AddAsync(notification);
            await unitOfWork.SaveAsync();

            foreach (var passenger in bookingsFrom)
            {
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
    }
}
