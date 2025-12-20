using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.IRepositries
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        IQueryable<TDto> GetBookings<TDto>();
    }
}
