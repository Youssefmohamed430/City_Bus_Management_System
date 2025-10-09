using City_Bus_Management_System.DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.IRepositries
{
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        IQueryable<TDto> FindAllSchedules<TDto>();
        TDto FindSchedulesByDriverId<TDto>(string id);
        TDto FindSchedulesByDriverName<TDto>(string Name);
        IQueryable<TDto> GetSchedulesByTripId<TDto>(int tripId);
    }
}
