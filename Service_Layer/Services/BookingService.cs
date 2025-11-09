
using City_Bus_Management_System.DataLayer.Entities;
using Data_Access_Layer.Helpers;
using System.Collections.Concurrent;

namespace Service_Layer.Services
{
    public class BookingService(IMemoryCache cache,IServiceScopeFactory _scopeFactory) : IBookingService
    {
        public Dictionary<int, int> CountOfBookings { get; set; } = new Dictionary<int, int>();
        private static readonly ConcurrentDictionary<int, object> TripLocks = new();
        public ResponseModel<BookingDTO> BookTicket(BookingDTO bookingdto)
        {
            string msg = "";
            bool isSuccess = true;
            var booking = bookingdto.Adapt<Booking>();

            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
                try
                {              
                    unitOfWork.BeginTransaction();

                    var ticket = unitOfWork.GetRepository<Ticket>().Find(t => t.Id == bookingdto.TicketId);

                    ValidateAvailableSeats(ticket,bookingdto.TripId);

                    unitOfWork.GetRepository<Booking>().AddAsync(booking);
                    unitOfWork.SaveAsync();

                    var isDeducted = walletService.DeductBalance(Convert.ToDouble(ticket!.Price),bookingdto.passengerId!);

                    if (!isDeducted)
                        throw new Exception("Failed to deduct wallet balance.");

                    cache.Remove("bookings");

                    unitOfWork.Commit();
                    msg = "Booking Successful and balance deducted!";
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    msg = ex.Message;
                    isSuccess = false;
                }
            }
            return ResponseModelFactory<BookingDTO>.CreateResponse(msg,isSuccess ? booking.Adapt<BookingDTO>() : null!,isSuccess);
        }

        private void ValidateAvailableSeats(Ticket Ticket, int TripId)
        {
            Schedule schedule;
            using (var scope = _scopeFactory.CreateScope())
            {
               var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

               schedule = unitOfWork.Schedules.FindAll(s => s.TripId == TripId &&
                    s.bus.BusType == Ticket.BusType, new string[] { "bus", "trip" })
                    .AsEnumerable()
                    .FirstOrDefault(s => EgyptTimeHelper.SplitFromUtc(s.DepartureDateTime).date == EgyptTimeHelper.TodayDateOnly)!;
            } 

            if (ValidateBookingTime(schedule))
                throw new Exception("Cannot book ticket for past departure time.");

            HandleCountOfBookings(schedule);
        }

        private void HandleCountOfBookings(Schedule schedule)
        {
            var tripId = schedule.trip!.Id;
            var lockObj = TripLocks.GetOrAdd(tripId, new object());

            lock (lockObj)
            {
                if (!CountOfBookings.ContainsKey(tripId))
                    CountOfBookings[tripId] = 0;

                if (CountOfBookings[tripId] >= schedule.bus!.TotalSeats)
                    throw new Exception("Booking limit reached.");

                CountOfBookings[tripId]++;
            }
        }

        private static bool ValidateBookingTime(Schedule schedule)
        {
            var scheduleEgyptTime = EgyptTimeHelper.ConvertFromUtc(schedule.DepartureDateTime);
            var nowEgyptTime = EgyptTimeHelper.Now;

            return nowEgyptTime >= scheduleEgyptTime.AddMinutes(10);
        }
        public ResponseModel<BookingDTO> CancelBooking(int bookingid)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

                var booking = unitOfWork.GetRepository<Booking>().Find(b => b.BookingId == bookingid, new string[] { "Ticket" });

                if (booking == null)
                    return ResponseModelFactory<BookingDTO>.CreateResponse("Booking not found", null!, false);

                try
                {
                    unitOfWork.BeginTransaction();

                    booking.Status = "Cancelled";

                    unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
                    unitOfWork.SaveAsync();

                    HandleCanceling(booking);

                    var isrefunded = walletService.RefundBalance(booking!.Ticket!.Price, booking.passengerId!);

                    if (!isrefunded)
                        throw new Exception("Failed to deduct wallet balance.");

                    cache.Remove("bookings");
                    unitOfWork.Commit();
                    return ResponseModelFactory<BookingDTO>.CreateResponse("Booking cancelled successfully", null!);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();

                    return ResponseModelFactory<BookingDTO>.CreateResponse(ex.Message, null!, false);
                }
            } 
        }

        private void HandleCanceling(Booking booking)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var Ticket = unitOfWork.GetRepository<Ticket>().Find(t => t.Id == booking.TicketId);

                var schedule = unitOfWork.Schedules.FindAll(s => s.TripId == booking.TripId &&
                    s.bus.BusType == Ticket.BusType, new string[] { "bus", "trip" })
                    .AsEnumerable()
                    .FirstOrDefault(s => EgyptTimeHelper.SplitFromUtc(s.DepartureDateTime).date == EgyptTimeHelper.TodayDateOnly)!;

                if (ValidateBookingTime(schedule))
                    throw new Exception("Cannot cancel booking for past departure time.");

                var tripId = booking.TripId;
                var lockObj = TripLocks.GetOrAdd(tripId, new object());

                lock (lockObj) { CountOfBookings[tripId]--; }
            }
        }

        public ResponseModel<List<BookingDTO>> GetBookings()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                if (!cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                {
                    bookings = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(_ => true, new string[] { "Trip", "Ticket", "passenger" }).ToList();
                    cache.Set("bookings", bookings, TimeSpan.FromMinutes(10));
                }
                return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings Fetched successfully", bookings, true);

            }
        }

        public ResponseModel<List<BookingDTO>> GetBookingsByPassengerId(string passengerid)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                List<BookingDTO> bookingByPassengerId = null!;

                if (cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                    bookingByPassengerId = bookings.Where(b => b.passengerId == passengerid).ToList()!;
                else
                    bookingByPassengerId = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(b => b.passengerId == passengerid, new string[] { "Trip", "Ticket", "passenger" }).ToList();

                return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings fetched successfully!", bookingByPassengerId);
            }
        }

        public ResponseModel<List<BookingDTO>> GetBookingsByRangeOfDate(DateTime start, DateTime end)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                List<BookingDTO> bookingByRangeOfDate = null!;

                if (cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                    bookingByRangeOfDate = bookings.Where(b => b.BookingDate >= start && b.BookingDate <= end).ToList()!;
                else
                    bookingByRangeOfDate = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(b => b.BookingDate >= start && b.BookingDate <= end, new string[] { "Trip", "Ticket", "passenger" }).ToList();

                return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings fetched successfully!", bookingByRangeOfDate);
            }
        }

        public ResponseModel<List<BookingDTO>> GetBookingsByTicketId(int ticketid)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                List<BookingDTO> bookingByTicketId = null!;

                if (cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                    bookingByTicketId = bookings.Where(b => b.TicketId == ticketid).ToList()!;
                else
                    bookingByTicketId = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(b => b.TicketId == ticketid, new string[] { "Trip", "Ticket", "passenger" }).ToList();

                return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings fetched successfully!", bookingByTicketId);
            }
        }

        public ResponseModel<List<BookingDTO>> GetBookingsByTripId(int tripid)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                List<BookingDTO> bookingByTripId = null!;

                if (cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                    bookingByTripId = bookings.Where(b => b.TripId == tripid).ToList()!;
                else
                    bookingByTripId = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(b => b.TripId == tripid, new string[] { "Trip", "Ticket", "passenger" }).ToList();

                return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings fetched successfully!", bookingByTripId);
            }
        }

        public ResponseModel<BookingDTO> UpdateBooking(int bookingid, BookingDTO Updatedbooking)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
                var booking = unitOfWork.GetRepository<Booking>().Find(b => b.BookingId == bookingid, new string[] { "Trip", "Ticket", "passenger" });

                if (booking == null)
                    return ResponseModelFactory<BookingDTO>.CreateResponse("Booking not found", null!, false);

                try
                {
                    booking.TicketId = Convert.ToInt32(Updatedbooking.TicketId);
                    booking.TripId = Convert.ToInt32(Updatedbooking.TicketId);

                    unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
                    unitOfWork.SaveAsync();
                    walletService.RefundBalance(booking!.Ticket!.Price, booking.passengerId!);
                    cache.Remove("bookings");
                    return ResponseModelFactory<BookingDTO>.CreateResponse("Booking cancelled successfully", null!);
                }
                catch (Exception ex)
                {
                    return ResponseModelFactory<BookingDTO>.CreateResponse(ex.Message, null!, false);
                }
            }
        }
        public StationDTO GetPassengerStartStationAsync(string passengerId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var StartStationbooking = unitOfWork.GetRepository<Booking>()
                    .Find(b => b.passengerId == passengerId,new string[] { "StationFrom" })
                    .StationFrom.Adapt<StationDTO>();

                return StartStationbooking;
            }
        }
    }
}
