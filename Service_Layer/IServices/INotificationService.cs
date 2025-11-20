using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.IServices
{
    public interface INotificationService
    {
        ResponseModel<List<Notification>> GetNotificationById(string id);
        Task NotifStationApproaching(List<Passenger> bookingsFrom,int dur,bool isfrom);
        Task SendNotification(string Id,string Msg);
    }
}
