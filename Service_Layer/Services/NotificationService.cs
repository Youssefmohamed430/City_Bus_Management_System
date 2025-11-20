using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Services;
using Data_Access_Layer.DataLayer.DTOs;
using Data_Access_Layer.Helpers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.Services
{
    public class NotificationService(IMemoryCache cache,IUnitOfWork unitOfWork,INotificationHubService notificationHub) : INotificationService
    {
        public ResponseModel<List<Notification>> GetNotificationById(string id)
        {
            if(!cache.TryGetValue($"Notification_{id}",out List<Notification> notification))
            {
                notification = unitOfWork.GetRepository<UserNotification>()
                    .FindAll(n => n.UserId == id,new string[] { "Notif", "User"})
                    .Select(n => n.Notif)
                    .ToList()!;
                if(notification == null || notification.Count <= 0)
                    return ResponseModelFactory<List<Notification>>.CreateResponse("Notification not found", null);
       
                cache.Set($"Notification_{id}", notification, TimeSpan.FromMinutes(10));
            }
            return ResponseModelFactory<List<Notification>>.CreateResponse("Notification retrieved successfully", notification);
        }

        public async Task NotifStationApproaching(List<Passenger> bookingsFrom,int dur,bool isfrom)
        {
            Notification notification = new Notification
            {
                Message =  isfrom ?
                $"your bus is approaching your boarding station remaining {dur} minutes to approach. Please be ready to board." :
                $"your bus is approaching your arrival station remaining {dur} minutes to arrive. Please be ready to go down.",

                Date = EgyptTimeHelper.ConvertToUtc(EgyptTimeHelper.Now)    
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

                await notificationHub.SendNotificationToUser(passenger.Id, new
                {
                    notifid = notification.NotifId,
                    msg     = notification.Message,
                    date    = notification.Date
                });
            }
            await unitOfWork.SaveAsync();
        }

        public async Task SendNotification(string Id, string Msg)
        {
            Notification notification = new Notification
            {
                Message = Msg,
                Date = EgyptTimeHelper.ConvertToUtc(EgyptTimeHelper.Now)
            };


            await unitOfWork.GetRepository<Notification>().AddAsync(notification);
            await unitOfWork.SaveAsync();

            UserNotification userNotification = new UserNotification
            {
                 UserId = Id,
                 NotifId = notification.NotifId,
            };

            await unitOfWork.GetRepository<UserNotification>().AddAsync(userNotification);

            await notificationHub.SendNotificationToUser(Id, new
            {
                  notifid = notification.NotifId,
                  msg = notification.Message,
                  date = notification.Date
            });

            await unitOfWork.SaveAsync();
        }
    }
}
