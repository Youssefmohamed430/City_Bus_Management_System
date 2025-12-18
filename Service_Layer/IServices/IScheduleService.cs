
namespace Service_Layer.IServices
{
    public interface IScheduleService
    {
        List<ScheduleDTO> GetSchedules();
        ResponseModel<List<ScheduleDTO>> GetSchedulesByDriverId(string Id);
        ResponseModel<List<ScheduleDTO>> GetSchedulesByDriverName(string Name);
        ResponseModel<List<ScheduleDTO>> GetSchedulesByTripId(int tripId);
        ResponseModel<ScheduleDTO> AddSchedule(ScheduleDTO driverSchedule);
        ResponseModel<ScheduleDTO> UpdateDriverSchedule(int SchId,ScheduleDTO newSchedule);
        ResponseModel<ScheduleDTO> RemoveDriverSchedule(int SchId);
        ScheduleDTO GetCurrentScheduleByDriverId(string Id);
    }
}
