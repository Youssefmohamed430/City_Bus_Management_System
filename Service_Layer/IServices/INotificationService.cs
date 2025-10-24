using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.IServices
{
    public interface INotificationService
    {
        Task<ResponseModel<Notification>> GetNotificationById(int id);
        Task NotifStationFromApproaching(List<Passenger> bookingsFrom);
        Task<ResponseModel<Notification>> NotifStationToApproaching(List<Passenger> bookingsTo);
    }
}
