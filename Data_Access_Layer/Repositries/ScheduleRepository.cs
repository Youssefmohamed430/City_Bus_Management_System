using City_Bus_Management_System.DataLayer.Data;
using Core_Layer.IRepositries;
using Data_Access_Layer.Helpers;
using Mapster;

namespace Data_Access_Layer.Repositries
{
    internal class ScheduleRepository : BaseRepository<Schedule>, IScheduleRepository
    {
        public AppDbContext Context { get; }

        public ScheduleRepository(AppDbContext context) : base(context)
        {
            Context = context;
        }

        public IQueryable<TDto> FindAllSchedules<TDto>()
        {
            var schedules = Context.Schedules.AsNoTracking()
                     .AsSplitQuery()
                     .Include(s => s.bus)
                     .Include(s => s.driver)
                     .ThenInclude(d => d.User)
                     .Include(s => s.trip)
                     .Where(s => !s.IsDeleted)
                     .ProjectToType<TDto>();

            return schedules;
        }

        public IQueryable<TDto> FindSchedulesByDriverId<TDto>(string id)
        {
            var schedule = Context.Schedules.AsNoTracking()
                     .AsSplitQuery()
                     .Include(s => s.bus)
                     .Include(s => s.driver)
                     .ThenInclude(d => d.User)
                     .Include(s => s.trip)
                     .Where(s => !s.IsDeleted && s.DriverId == id)
                     .ProjectToType<TDto>();

            return schedule;
        }

        public IQueryable<TDto> FindSchedulesByDriverName<TDto>(string Name)
        {
            var schedule = Context.Schedules.AsNoTracking()
                     .AsSplitQuery()
                     .Include(s => s.bus)
                     .Include(s => s.driver)
                     .ThenInclude(d => d.User)
                     .Include(s => s.trip)
                     .Where(s => !s.IsDeleted && s.driver.User.Name.ToLower() == Name.ToLower())
                     .ProjectToType<TDto>();

            return schedule;
        }
        public IQueryable<TDto> GetSchedulesByTripId<TDto>(int tripId)
        {
            var schedules = Context.Schedules.AsNoTracking()
                     .AsSplitQuery()
                     .Include(s => s.bus)
                     .Include(s => s.driver)
                     .ThenInclude(d => d.User)
                     .Include(s => s.trip)
                     .Where(s => !s.IsDeleted && s.TripId == tripId)
                     .ProjectToType<TDto>();

            return schedules;
        }

        public TDto GetCurrentScheduleByDriverId<TDto>(string Id)
        {
            TimeZoneInfo egyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            DateTime egyptNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptZone);

            var schedules = Context.Schedules.AsNoTracking()
                     .AsSplitQuery()
                     .Include(s => s.bus)
                     .Include(s => s.driver)
                     .ThenInclude(d => d.User)
                     .Include(s => s.trip)
                     .Where(s => s.DriverId == Id && EgyptTimeHelper.ConvertFromUtc(s.DepartureDateTime) == EgyptTimeHelper.Now)
                     .ProjectToType<TDto>()
                     .FirstOrDefault()!;

            return schedules;
        }
    }
}
