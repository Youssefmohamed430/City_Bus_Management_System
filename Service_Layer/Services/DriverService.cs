using Core_Layer.Entities;
using Data_Access_Layer.Helpers;

namespace Service_Layer.Services
{
    public class DriverService(IUnitOfWork unitOfWork) : IDriverService
    {
        public ResponseModel<object> UpdateTripStatus(string driverId,string Status)
        {
            var DriverData = unitOfWork.GetRepository<DriverStatistics>().Find(d => d.DriverId == driverId);

            if (DriverData == null)
            {
                DriverData = CreateDriverStats(driverId);
            }
            string msg = "";

            if (Status == "Start")
            {
                DriverData.TotalTrips++;
                DriverData.UpdateTime = EgyptTimeHelper.ConvertToUtc(EgyptTimeHelper.Now);
                // Send Notification to the user and Admin that the trip has Started
                msg = "Trip Started Successfully";
            }
            else if (Status == "Cancel")
            {
                DriverData.CancelledTrips++;
                // Send Notification to the user and Admin that the trip has Cancelled
                msg = "Trip Cancelled Successfully";
            }
            else if (Status == "End")
            {
                if(DriverData.UpdateTime == null! || (EgyptTimeHelper.Now - DriverData.UpdateTime).TotalHours < 2)
                    return new ResponseModel<object>() { Message = "Trip cannot be ended before 2 hours of start time", Result = null! };
                DriverData.CompletedTrips++;
                var schedule = 
                // Send Notification to the user and Admin that the trip has Completed
                msg = "Trip Completed Successfully";
            }
            else
            {
                return new ResponseModel<object>() { Message = "Invalid Status", Result = null! };
            }
            unitOfWork.GetRepository<DriverStatistics>().UpdateAsync(DriverData);
            unitOfWork.SaveAsync();
            return new ResponseModel<object>() { Message = msg, Result = null!};
        }

        private DriverStatistics CreateDriverStats(string driverId)
        {
            DriverStatistics DriverData;
            var DriverStats = new DriverStatistics()
            {
                DriverId = driverId,
                TotalTrips = 0,
                CompletedTrips = 0,
                CancelledTrips = 0
            };

            unitOfWork.GetRepository<DriverStatistics>().AddAsync(DriverStats);
            unitOfWork.SaveAsync();
            DriverData = DriverStats;
            return DriverData;
        }
    }
}
