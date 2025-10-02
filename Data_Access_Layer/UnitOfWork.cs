using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer;
using Core_Layer.IRepositries;
using Data_Access_Layer.Repositries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IBaseRepository<Bus> Buses { get; private set; }

        public IScheduleRepository Schedules { get; private set; }
        public IBaseRepository<Trip> Trips { get; private set; }
        public IBaseRepository<DriverRequests> DriverReqs { get; private set; }
        public IBaseRepository<Driver> Drivers { get; private set; }
        public IBaseRepository<Station> Stations { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Buses = new BaseRepository<Bus>(_context);
            Trips = new BaseRepository<Trip>(_context);
            Schedules = new ScheduleRepository(_context);
            DriverReqs = new BaseRepository<DriverRequests>(_context);
            Drivers = new BaseRepository<Driver>(_context);
            Stations = new BaseRepository<Station>(_context);
        }

        public async Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync().Result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
