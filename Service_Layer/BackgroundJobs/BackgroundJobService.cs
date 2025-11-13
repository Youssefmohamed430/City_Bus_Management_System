using Data_Access_Layer.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.BackgroundJobs
{
    public class BackgroundJobService(
        UserManager<ApplicationUser> _userManager , IUnitOfWork _unitOfWork,
        ILogger<BackgroundJobService> logger , IMemoryCache _cache
    ) : IBackgroundJobService
    {

        public async Task CleanExpiredRefreshTokens()
        {
            logger.LogInformation("Starting RefreshTokens Clean...");

            var users = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .ToListAsync();

            foreach (var user in users)
            {
                user.RefreshTokens.RemoveAll(t => t.IsExpired);
                await _userManager.UpdateAsync(user);
            }

            logger.LogInformation("RefreshTokens Clean completed");
        }
        public async Task RefreshAllCaches()
        {
            logger.LogInformation("Starting cache refresh...");

            // Trips
            var trips = await _unitOfWork.GetRepository<Trip>()
                .FindAll<TripDTO>(t => !t.IsDeleted).ToListAsync();
            _cache.Set("trips", trips, TimeSpan.FromMinutes(20));

            // Stations
            var stations = await _unitOfWork.GetRepository<Station>()
                .FindAll<StationDTO>(s => !s.IsDeleted).ToListAsync();
            _cache.Set("stations", stations, TimeSpan.FromMinutes(15));

            // Schedules
            var schedules = _unitOfWork.Schedules
                .FindAllSchedules<ScheduleDTO>().ToList();
            _cache.Set("schedules", schedules, TimeSpan.FromMinutes(15));

            // Bookings
            var bookings = _unitOfWork.GetRepository<Booking>()
                .FindAll<BookingDTO>(_ => true, new[] { "Trip", "Ticket", "passenger" })
                .ToList();
            _cache.Set("bookings", bookings, TimeSpan.FromMinutes(10));

            logger.LogInformation("Cache refresh completed");
        }
    }
}
