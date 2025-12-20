using City_Bus_Management_System.DataLayer.Data;
using Core_Layer.IRepositries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositries
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public AppDbContext _context { get; set; }
        public BookingRepository(AppDbContext appDbContext) : base(appDbContext) 
        { this._context = appDbContext; }
        public IQueryable<TDto> GetBookings<TDto>()
        {
            var bookings = _context.Database
                .SqlQueryRaw<TDto>("SELECT * FROM FullBookingsInfo");

            return bookings;
        }
    }
}
