using City_Bus_Management_System.DataLayer;
using Core_Layer;
using Core_Layer.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Service_Layer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.Services
{
    public class DriverService : IDriverService
    {
        public IUnitOfWork unitOfWork { get; set; }
        public DriverService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public ResponseModel<object> UpdateTripStatus(string driverId,string Status)
        {
            var DriverData = unitOfWork.DriverStatistics.Find(d => d.DriverId == driverId);

            if (DriverData == null)
            {
                var DriverStats = new DriverStatistics()
                {
                    DriverId = driverId,
                    TotalTrips = 0,
                    CompletedTrips = 0,
                    CancelledTrips = 0
                };

                unitOfWork.DriverStatistics.AddAsync(DriverStats);
                unitOfWork.SaveAsync();
                DriverData = DriverStats;  
            }
            string msg = "";

            if (Status == "Start")
            {
                DriverData.TotalTrips++;
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
                DriverData.CompletedTrips++;
                // Send Notification to the user and Admin that the trip has Completed
                msg = "Trip Completed Successfully";
            }
            else
            {
                return new ResponseModel<object>() { Message = "Invalid Status", Result = null! };
            }
            unitOfWork.DriverStatistics.UpdateAsync(DriverData);
            unitOfWork.SaveAsync();
            return new ResponseModel<object>() { Message = msg, Result = null!};
        }
    }
}
