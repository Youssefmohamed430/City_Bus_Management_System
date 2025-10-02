using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer.IRepositries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer
{
    public interface IUnitOfWork : IDisposable
    {
        public IBaseRepository<Bus> Buses { get; }
        public IScheduleRepository Schedules { get; }
        public IBaseRepository<Trip> Trips { get; }
        public IBaseRepository<DriverRequests> DriverReqs { get; }
        public IBaseRepository<Driver> Drivers { get; }
        public IBaseRepository<Station> Stations { get; }

        Task<int> SaveAsync();
    }
}
