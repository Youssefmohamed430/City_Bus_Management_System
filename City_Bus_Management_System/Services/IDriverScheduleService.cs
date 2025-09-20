using Azure;
using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;

namespace City_Bus_Management_System.Services
{
    public interface IDriverScheduleService
    {
        ResponseModel<List<ScheduleDTO>> GetSchedules();
        ResponseModel<ScheduleDTO> GetSchedulesByDriverId(string Id);
        ResponseModel<ScheduleDTO> GetSchedulesByDriverName(string Name);
        ResponseModel<ScheduleDTO> AddSchedule(ScheduleDTO driverSchedule);
        ResponseModel<ScheduleDTO> UpdateDriverSchedule(int SchId,ScheduleDTO newSchedule);
        ResponseModel<ScheduleDTO> RemoveDriverSchedule(int SchId);
    }
}
