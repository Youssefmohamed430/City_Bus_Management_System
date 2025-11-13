using Hangfire;
using Service_Layer.BackgroundJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.ServiceRegistration
{
    public static class BackgroundJobsAddition
    {
        public static void AddBackgroundJobServices()
        {
            //RecurringJob.AddOrUpdate<IBackgroundJobService>(
            //    "clean-booking-counters",
            //    service => service.CleanOldBookingCounts(),
            //    Cron.Hourly);

            RecurringJob.AddOrUpdate<IBackgroundJobService>(
                "refresh-all-caches",
                service => service.RefreshAllCaches(),
                "*/30 * * * *"); // Every 30 minutes

            RecurringJob.AddOrUpdate<IBackgroundJobService>(
                "clean-expired-refresh-tokens",
                service => service.CleanExpiredRefreshTokens(),
                Cron.Daily); 
        }
    }
}
