
namespace Service_Layer.IServices
{
    public interface IScheduleService
    {
        ResponseModel<List<ScheduleDTO>> GetSchedules();
        ResponseModel<ScheduleDTO> GetSchedulesByDriverId(string Id);
        ResponseModel<ScheduleDTO> GetSchedulesByDriverName(string Name);
        ResponseModel<List<ScheduleDTO>> GetSchedulesByTripId(int tripId);
        ResponseModel<ScheduleDTO> AddSchedule(ScheduleDTO driverSchedule);
        ResponseModel<ScheduleDTO> UpdateDriverSchedule(int SchId,ScheduleDTO newSchedule);
        ResponseModel<ScheduleDTO> RemoveDriverSchedule(int SchId);
        ScheduleDTO GetCurrentScheduleByDriverId(string Id);
    }
}
