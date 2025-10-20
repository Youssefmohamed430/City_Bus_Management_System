using City_Bus_Management_System.DataLayer.Entities;

namespace Core_Layer.IRepositries
{
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        IQueryable<TDto> FindAllSchedules<TDto>();
        TDto FindSchedulesByDriverId<TDto>(string id);
        TDto FindSchedulesByDriverName<TDto>(string Name);
        IQueryable<TDto> GetSchedulesByTripId<TDto>(int tripId);
        TDto GetCurrentScheduleByDriverId<TDto>(string Id);
    }
}
