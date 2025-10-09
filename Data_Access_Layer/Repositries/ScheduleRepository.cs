using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer.IRepositries;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
                     .Include(s => s.bus)
                     .Include(s => s.driver)
                     .ThenInclude(d => d.User)
                     .Include(s => s.trip)
                     .Where(s => !s.IsDeleted)
                     .ProjectToType<TDto>();

            return schedules;
        }

        public TDto FindSchedulesByDriverId<TDto>(string id)
        {
            var schedule = Context.Schedules.AsNoTracking()
                     .Include(s => s.bus)
                     .Include(s => s.driver)
                     .ThenInclude(d => d.User)
                     .Include(s => s.trip)
                     .Where(s => !s.IsDeleted && s.DriverId == id)
                     .ProjectToType<TDto>()
                     .FirstOrDefault()!;

            return schedule;
        }

        public TDto FindSchedulesByDriverName<TDto>(string Name)
        {
            var schedule = Context.Schedules.AsNoTracking()
                     .Include(s => s.bus)
                     .Include(s => s.driver)
                     .ThenInclude(d => d.User)
                     .Include(s => s.trip)
                     .Where(s => !s.IsDeleted && s.driver.User.Name.ToLower() == Name.ToLower())
                     .ProjectToType<TDto>()
                     .FirstOrDefault()!;

            return schedule;
        }
        public IQueryable<TDto> GetSchedulesByTripId<TDto>(int tripId)
        {
            var schedules = Context.Schedules.AsNoTracking()
                     .Include(s => s.bus)
                     .Include(s => s.driver)
                     .ThenInclude(d => d.User)
                     .Include(s => s.trip)
                     .Where(s => !s.IsDeleted && s.TripId == tripId)
                     .ProjectToType<TDto>();

            return schedules;
        }
    }
}
