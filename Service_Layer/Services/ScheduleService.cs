
using Data_Access_Layer.Helpers;

namespace City_Bus_Management_System.Services
{
    public class ScheduleService(IMemoryCache cache, ILogger<BusService> logger, IUnitOfWork unitofWork) : IScheduleService
    {

        public ResponseModel<List<ScheduleDTO>> GetSchedules()
        {
            if(!cache.TryGetValue("schedules", out List<ScheduleDTO> schedules))
            {
                schedules = unitofWork.Schedules
                    .FindAllSchedules<ScheduleDTO>().ToList();

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
        public ResponseModel<List<ScheduleDTO>> GetSchedulesByDriverId(string Id)
        {
            List<ScheduleDTO> schedulesByDriverId = null!;

            if (cache.TryGetValue("schedules", out List<ScheduleDTO> schedules))
                schedulesByDriverId = schedules
                    .Where(s => s.DriverId == Id)!.ToList();
            else
                schedulesByDriverId = unitofWork.Schedules
                    .FindSchedulesByDriverId<ScheduleDTO>(Id).ToList();

            return new ResponseModel<List<ScheduleDTO>>
            {
                IsSuccess = true,
                Message = "Schedules By DriverId fetched successfully",
                Result = schedulesByDriverId
            };
        }
        public ScheduleDTO GetCurrentScheduleByDriverId(string Id)
        {
            ScheduleDTO schedulesByDriverId = null!;

            if (cache.TryGetValue("schedules", out List<ScheduleDTO> schedules))
                schedulesByDriverId = schedules
                    .FirstOrDefault(s => s.DriverId == Id &&
                            EgyptTimeHelper.ConvertFromUtc(s.DepartureDateTime).Date == EgyptTimeHelper.Now.Date &&
                            EgyptTimeHelper.ConvertFromUtc(s.DepartureDateTime).Hour == EgyptTimeHelper.Now.Hour)!;
            else
                schedulesByDriverId = unitofWork.Schedules
                    .GetCurrentScheduleByDriverId<ScheduleDTO>(Id);

            return schedulesByDriverId;
        }
        public ResponseModel<List<ScheduleDTO>> GetSchedulesByDriverName(string DriverName)
        {
            List<ScheduleDTO> schedulesByDriverName = null!;

            if (cache.TryGetValue("schedules", out List<ScheduleDTO> schedules))
            {
                schedulesByDriverName = schedules
                    .Where(s => s.DriverName.ToLower() == DriverName.ToLower())!.ToList();
            }
            else
            {
                schedulesByDriverName = unitofWork.Schedules
                    .FindSchedulesByDriverName<ScheduleDTO>(DriverName).ToList();
            }

            return new ResponseModel<List<ScheduleDTO>>
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
                unitofWork.Schedules.AddAsync(schedule);

                unitofWork.SaveAsync();

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
            var schedule = unitofWork.Schedules.Find(x => x.SchId == SchId);

            schedule.DepartureDateTime = EgyptTimeHelper.ConvertToUtc(newSchedule.DepartureDateTime);
            schedule.BusId = Convert.ToInt32(newSchedule.BusId);
            schedule.DriverId = newSchedule.DriverId;
            schedule.TripId = Convert.ToInt32(newSchedule.TripId);

            try
            {
                unitofWork.Schedules.UpdateAsync(schedule);
                unitofWork.SaveAsync();
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
            var schedule = unitofWork.Schedules.Find(x =>x.SchId == SchId);

            try
            {
                schedule.IsDeleted = true;

                unitofWork.Schedules.UpdateAsync(schedule);
                unitofWork.SaveAsync();
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

        public ResponseModel<List<ScheduleDTO>> GetSchedulesByTripId(int tripId)
        {
            var SchedulesByTripId = unitofWork.Schedules.GetSchedulesByTripId<ScheduleDTO>(tripId);

            return ResponseModelFactory<List<ScheduleDTO>>.CreateResponse("Schedules fetched successfully!", SchedulesByTripId.ToList());
        }
    }
}
