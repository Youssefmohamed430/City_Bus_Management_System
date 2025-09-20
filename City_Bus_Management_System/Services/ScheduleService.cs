using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace City_Bus_Management_System.Services
{
    public class ScheduleService : IDriverScheduleService
    {
        private readonly IMemoryCache cache;
        private readonly AppDbContext context;

        public ScheduleService(AppDbContext context,IMemoryCache _cache)
        {
            this.context = context;
            this.cache = _cache;
        }

        public ResponseModel<List<ScheduleDTO>> GetSchedules()
        {
            throw new NotImplementedException();
        }

        public ResponseModel<ScheduleDTO> GetSchedulesByDriverId()
        {
            throw new NotImplementedException();
        }

        public ResponseModel<ScheduleDTO> GetSchedulesByDriverName()
        {
            throw new NotImplementedException();
        }

        public void AssignDriverToBus(ScheduleDTO driverSchedule)
        {
            throw new NotImplementedException();
        }

        public void UpdateDriverSchedule(int SchId, string newSchedule)
        {
            throw new NotImplementedException();
        }

        public void RemoveDriverSchedule(int SchId)
        {
            throw new NotImplementedException();
        }
    }
}
