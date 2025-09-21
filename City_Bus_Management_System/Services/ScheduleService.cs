using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace City_Bus_Management_System.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IMemoryCache cache;
        private readonly AppDbContext context;
        private ILogger<BusService> logger;

        public ScheduleService(AppDbContext context,IMemoryCache _cache, ILogger<BusService> logger)
        {
            this.context = context;
            this.cache = _cache;
            this.logger = logger;
        }

        public ResponseModel<List<ScheduleDTO>> GetSchedules()
        {
            if(!cache.TryGetValue("schedules", out List<ScheduleDTO> schedules))
            {
                schedules = context.Schedules
                    .AsNoTracking()
                    .Include(s => s.bus)
                    .Include(s => s.driver)
                    .ThenInclude(d => d.User)
                    .Include(s => s.trip)
                    .Where(s => !s.IsDeleted)
                    .ProjectToType<ScheduleDTO>()
                    .ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
                                .SetPriority(CacheItemPriority.Normal);

                cache.Set("schedules", schedules, cacheEntryOptions);
            }
               
            return new ResponseModel<List<ScheduleDTO>>
            {
                IsSuccess = true,
                Message = "Schedules fetched successfully",
                Result = schedules
            };
        }
        public ResponseModel<ScheduleDTO> GetSchedulesByDriverId(string Id)
        {
            ScheduleDTO schedulesByDriverId = null!;

            if (cache.TryGetValue("schedules", out List<ScheduleDTO> schedules))
            {
                schedulesByDriverId = schedules
                    .FirstOrDefault(s => s.DriverId == Id)!;
            }
            else
            {
                schedulesByDriverId = context.Schedules
                    .AsNoTracking()
                    .Include(s => s.bus)
                    .Include(s => s.driver)
                    .ThenInclude(d => d.User)
                    .Include(s => s.trip)
                    .Where(s => !s.IsDeleted && s.DriverId == Id)
                    .ProjectToType<ScheduleDTO>()
                    .FirstOrDefault()!;
            }

            return new ResponseModel<ScheduleDTO>
            {
                IsSuccess = true,
                Message = "Schedules By DriverId fetched successfully",
                Result = schedulesByDriverId
            };
        }
        public ResponseModel<ScheduleDTO> GetSchedulesByDriverName(string DriverName)
        {
            ScheduleDTO schedulesByDriverName = null!;

            if (cache.TryGetValue("schedules", out List<ScheduleDTO> schedules))
            {
                schedulesByDriverName = schedules
                    .FirstOrDefault(s => s.DriverName.ToLower() == DriverName.ToLower())!;
            }
            else
            {
                schedulesByDriverName = context.Schedules
                    .AsNoTracking()
                    .Include(s => s.bus)
                    .Include(s => s.driver)
                    .ThenInclude(d => d.User)
                    .Include(s => s.trip)
                    .Where(s => !s.IsDeleted && s.driver.User.Name.ToLower() == DriverName.ToLower())
                    .ProjectToType<ScheduleDTO>()
                    .FirstOrDefault()!;
            }

            return new ResponseModel<ScheduleDTO>
            {
                IsSuccess = true,
                Message = "Schedules By Driver Name fetched successfully",
                Result = schedulesByDriverName
            };
        }
        public ResponseModel<ScheduleDTO> AddSchedule(ScheduleDTO Schedule)
        {
            var schedule = Schedule.Adapt<Schedule>();

            try
            {
                context.Schedules.Add(schedule);

                context.SaveChanges();

                cache.Remove("schedules");

                return new ResponseModel<ScheduleDTO>
                {
                    Message = "Schedule added successfully",
                    Result = Schedule,
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding Schedule");

                return new ResponseModel<ScheduleDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = null!
                };
            }
        }
        public ResponseModel<ScheduleDTO> UpdateDriverSchedule(int SchId, ScheduleDTO newSchedule)
        {
            var schedule = context.Schedules.FirstOrDefault(x => x.SchId == SchId);

            schedule.DepartureTime = newSchedule.DepartureTime;
            schedule.BusId = Convert.ToInt32(newSchedule.BusId);
            schedule.DriverId = newSchedule.DriverId;
            schedule.TripId = Convert.ToInt32(newSchedule.TripId);

            try
            {
                context.Schedules.Update(schedule);
                context.SaveChanges();
                logger.LogInformation("Schedule updated successfully");
                cache.Remove("schedules");

                return new ResponseModel<ScheduleDTO>
                {
                    IsSuccess = true,
                    Message = "Schedule updated successfully",
                    Result = newSchedule
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating Schedule");

                return new ResponseModel<ScheduleDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = null!
                };
            }
        }
        public ResponseModel<ScheduleDTO> RemoveDriverSchedule(int SchId)
        {
            var schedule = context.Schedules.FirstOrDefault(x =>x.SchId == SchId);

            try
            {
                schedule.IsDeleted = true;

                context.Schedules.Update(schedule);

                context.SaveChanges();
                cache.Remove("schedules");

                logger.LogInformation("Schedule removed successfully");

                return new ResponseModel<ScheduleDTO>
                {
                    IsSuccess = true,
                    Message = "Schedule removed successfully",
                    Result = null!
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing Schedule");

                return new ResponseModel<ScheduleDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = null!
                };
            }
        }
    }
}
