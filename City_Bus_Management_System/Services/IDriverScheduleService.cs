using Azure;
using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;

namespace City_Bus_Management_System.Services
{
    public interface IDriverScheduleService
    {
        ResponseModel<List<DriverScheduleDTO>> GetSchedules();
        ResponseModel<DriverScheduleDTO> GetSchedulesByDriverId();
        ResponseModel<DriverScheduleDTO> GetSchedulesByDriverName();
        void AssignDriverToBus(DriverScheduleDTO driverSchedule);
        void UpdateDriverSchedule(int SchId, string newSchedule);
        void RemoveDriverSchedule(int SchId);
    }
}
