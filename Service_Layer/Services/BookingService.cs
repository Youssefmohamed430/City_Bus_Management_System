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
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<BookingService>>();
                try
                {              
                    unitOfWork.BeginTransaction();

                    var ticket = unitOfWork.GetRepository<Ticket>().Find(t => t.Id == bookingdto.TicketId);

                    if(ticket == null)
                        throw new Exception("Ticket not found.");

                    ValidateAvailableSeats(ticket,bookingdto.TripId
                        ,Convert.ToInt32(bookingdto.numberOfTickets));

                    unitOfWork.GetRepository<Booking>().AddAsync(booking);
                    unitOfWork.SaveAsync();

                    booking.totalPrice = Convert.ToInt32(ticket!.Price * bookingdto.numberOfTickets);

                    var isDeducted = walletService.DeductBalance(booking.totalPrice,bookingdto.passengerId!);

                    if (!isDeducted)
                        throw new Exception("Failed to deduct wallet balance.");

                    cache.Remove("bookings");

                    notificationService.SendNotification(bookingdto.passengerId!,
                        $"The ticket for the trip has been successfully booked and balance deducted.");

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

        private void ValidateAvailableSeats(Ticket Ticket, int TripId,int numOfTickets)
        {
            Schedule schedule;
            using (var scope = _scopeFactory.CreateScope())
            {
               var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
               var logger = scope.ServiceProvider.GetRequiredService<ILogger<BookingService>>();

                schedule = unitOfWork.Schedules.FindAll(s => s.TripId == TripId &&
                    s.bus.BusType == Ticket.BusType, new string[] { "bus", "trip" })
                    .AsEnumerable()
                    .FirstOrDefault(s =>
                    {
                        var dep = EgyptTimeHelper.ConvertFromUtc(s.DepartureDateTime);
                        var now = EgyptTimeHelper.Now;

                        return dep > now;
                    })!;

                if (schedule == null)
                    throw new Exception("No upcoming schedule found for the specified trip and bus type.");
            } 

            if (ValidateBookingTime(schedule))
                throw new Exception("Cannot book ticket for past departure time.");

            HandleCountOfBookings(schedule,numOfTickets);
        }

        private void HandleCountOfBookings(Schedule schedule,int numOfTickets)
        {
            var tripId = schedule.trip!.Id;
            var lockObj = TripLocks.GetOrAdd(tripId, new object());

            lock (lockObj)
            {
                if (!CountOfBookings.ContainsKey(tripId))
                    CountOfBookings[tripId] = 0;

                if (CountOfBookings[tripId] + numOfTickets > schedule.bus!.TotalSeats)
                    throw new Exception("No available seats.");

                CountOfBookings[tripId] += numOfTickets; 
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
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var booking = unitOfWork.GetRepository<Booking>().Find(
                            b => b.BookingId == bookingid && b.Status != "Cancelled",
                            new string[] { "Ticket", "Trip", "Trip.Schedules", "Trip.Schedules.bus" });

                if (booking == null)
                    return ResponseModelFactory<BookingDTO>.CreateResponse(
                        "Booking not found or already cancelled", null!, false);

                try
                {
                    unitOfWork.BeginTransaction();

                    booking.Status = "Cancelled";

                    unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
                    unitOfWork.SaveAsync();

                    HandleCanceling(booking,unitOfWork);

                    var isrefunded = walletService.RefundBalance(booking!.totalPrice, booking.passengerId!);

                    if (!isrefunded)
                        throw new Exception("Failed to deduct wallet balance.");

                    cache.Remove("bookings");

                    var msg = $"The ticket for the trip has been successfully cancelled and balance refunded.";
                    notificationService.SendNotification(booking.passengerId, msg);

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

        private void HandleCanceling(Booking booking,IUnitOfWork unitOfWork)
        {
            var ticket = booking.Ticket ?? unitOfWork.GetRepository<Ticket>()
                .Find(t => t.Id == booking.TicketId);

            var schedule = booking.Trip?.Schedules?
                .FirstOrDefault(s =>
                {
                    var dep = EgyptTimeHelper.ConvertFromUtc(s.DepartureDateTime);
                    var now = EgyptTimeHelper.Now;

                    return dep > now;
                })!;


            if (schedule == null)
            {
                schedule = unitOfWork.Schedules.FindAll(
                    s => s.TripId == booking.TripId && s.bus.BusType == ticket.BusType,
                    new string[] { "bus", "trip" })
                    .AsEnumerable()
                    .FirstOrDefault(s =>
                    {
                        var dep = EgyptTimeHelper.ConvertFromUtc(s.DepartureDateTime);
                        var now = EgyptTimeHelper.Now;

                        return dep > now;
                    })!;
            }

            if (schedule == null)
                throw new Exception("No upcoming schedule found for the specified trip and bus type.");

            if (ValidateBookingTime(schedule))
                    throw new Exception("Cannot cancel booking for past departure time.");

                var tripId = booking.TripId;
                var lockObj = TripLocks.GetOrAdd(tripId, new object());

                lock (lockObj) { CountOfBookings[tripId] -= booking.numberOfTickets; }
        }

        public List<BookingDTO> GetBookings()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                //var bookings = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(_ => true, new string[] { "Trip", "Ticket", "passenger" }).ToList();
                var bookings = unitOfWork.Bookings.GetBookings<BookingDTO>().ToList();

                return  bookings;
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

                if(start > end)
                    return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Invalid date range!", null!, false);

                if(start > EgyptTimeHelper.Now)
                    return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Start date cannot be in the future!", null!, false);

                if (cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                    bookingByRangeOfDate = bookings.Where(b => b.BookingDate >= start && b.BookingDate <= end).ToList()!;
                else
                    bookingByRangeOfDate = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(b => b.BookingDate >= start && b.BookingDate <= end, new string[] { "Trip", "Ticket", "passenger" }).ToList();

                if(bookingByRangeOfDate == null|| bookingByRangeOfDate.Count == 0)
                    return ResponseModelFactory<List<BookingDTO>>.CreateResponse("No bookings found in the specified date range!", null!, false);

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

                if (bookingByTicketId == null || bookingByTicketId.Count == 0)
                    return ResponseModelFactory<List<BookingDTO>>.CreateResponse("No bookings found with this ticket.", null!, false);

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
                
                if (bookingByTripId == null || bookingByTripId.Count == 0)
                    return ResponseModelFactory<List<BookingDTO>>.CreateResponse("No bookings found with this trip.", null!, false);

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
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                if (booking == null)
                    return ResponseModelFactory<BookingDTO>.CreateResponse("Booking not found", null!, false);

                try
                {
                    booking.TicketId = Convert.ToInt32(Updatedbooking.TicketId);
                    booking.totalPrice = Convert.ToInt32(booking.Ticket.Price * booking.numberOfTickets);
                    booking.TripId = Convert.ToInt32(Updatedbooking.TripId);
                    booking.StationFromId = Updatedbooking.StationFromId;
                    booking.StationToId = Updatedbooking.StationToId;

                    unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
                    unitOfWork.SaveAsync();
                    walletService.RefundBalance(booking!.Ticket!.Price, booking.passengerId!);
                    cache.Remove("bookings");

                    var msg = $"The ticket for the trip has been successfully Updated.";

                    notificationService.SendNotification(booking.passengerId, msg);

                    return ResponseModelFactory<BookingDTO>.CreateResponse("Booking Updated successfully", null!);
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
