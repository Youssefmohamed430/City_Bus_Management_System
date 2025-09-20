using Azure;
using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;

namespace City_Bus_Management_System.Services
{
    public interface IDriverScheduleService
    {
        ResponseModel<List<ScheduleDTO>> GetSchedules();
        ResponseModel<ScheduleDTO> GetSchedulesByDriverId();
        ResponseModel<ScheduleDTO> GetSchedulesByDriverName();
        void AssignDriverToBus(ScheduleDTO driverSchedule);
        void UpdateDriverSchedule(int SchId, string newSchedule);
        void RemoveDriverSchedule(int SchId);
    }
}
