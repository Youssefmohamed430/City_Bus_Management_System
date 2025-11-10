using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.BackgroundJobs
{
    public interface IBackgroundJobService
    {
        Task CleanExpiredRefreshTokens();
        Task RefreshAllCaches();
    }
}
