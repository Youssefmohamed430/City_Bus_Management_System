using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer;
using Data_Access_Layer.DataLayer.DTOs;
using Data_Access_Layer.Factories;
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using Service_Layer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.Services
{
    public class BookingService(IUnitOfWork unitOfWork, IWalletService walletService,
        IMemoryCache cache, IScheduleService scheduleService) : IBookingService
    {

        public ResponseModel<BookingDTO> BookTicket(BookingDTO bookingdto)
        {
            string msg = "";
            bool isSuccess = true;
            var booking = bookingdto.Adapt<Booking>();

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

            return ResponseModelFactory<BookingDTO>.CreateResponse(msg,isSuccess ? booking.Adapt<BookingDTO>() : null!,isSuccess);
        }

        private void ValidateAvailableSeats(Ticket Ticket,int TripId)
        {
            var schedule = unitOfWork.Schedules.Find(s => s.TripId == TripId && s.bus.BusType == Ticket.BusType, new string[] { "bus", "trip" });

            if (ValidateBookingTime(schedule))
                throw new Exception("Cannot book ticket for past departure time.");

            DateTime expiryTime = Convert.ToDateTime(schedule.DepartureTime).AddMinutes(10);

            var cacheKey = $"CountOfBookings[{schedule.trip!.Id}]";

            cache.TryGetValue(cacheKey, out int countOfBookings);

            if (countOfBookings >= schedule.bus!.TotalSeats)
                throw new Exception("Booking limit reached. Cannot book more tickets at this time.");

            countOfBookings++;
            cache.Set(cacheKey, countOfBookings, new MemoryCacheEntryOptions { AbsoluteExpiration = expiryTime });

        }

        private static bool ValidateBookingTime(Schedule schedule)
            => DateTime.Now.TimeOfDay > schedule.DepartureTime;

        public ResponseModel<BookingDTO> CancelBooking(int bookingid)
        {
            var booking = unitOfWork.GetRepository<Booking>().Find(b => b.BookingId == bookingid,new string[] {"Ticket"});

            if (booking == null)
                return ResponseModelFactory<BookingDTO>.CreateResponse("Booking not found", null!, false);

            try
            {
                unitOfWork.BeginTransaction();

                booking.Status = "Cancelled";

                unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
                unitOfWork.SaveAsync();

                HandleCancelingAtCache(booking);

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

        private void HandleCancelingAtCache(Booking booking)
        {
            var Ticket = unitOfWork.GetRepository<Ticket>().Find(t => t.Id == booking.TicketId);

            var schedule = unitOfWork.Schedules.Find(s => s.TripId == booking.TripId && s.bus.BusType == Ticket.BusType, new string[] { "bus", "trip" });

            if(ValidateBookingTime(schedule))
                throw new Exception("Cannot cancel booking for past departure time.");

            DateTime expiryTime = Convert.ToDateTime(schedule.DepartureTime).AddMinutes(5);

            var cacheKey = $"CountOfBookings[{booking.TripId}]";

            cache.TryGetValue(cacheKey, out int countOfBookings);

            countOfBookings--;

            cache.Set(cacheKey, countOfBookings, new MemoryCacheEntryOptions { AbsoluteExpiration = expiryTime });
        }

        public ResponseModel<List<BookingDTO>> GetBookings()
        {
            if(!cache.TryGetValue("bookings", out List<BookingDTO> bookings))
            {
                bookings = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(_ => true,new string[] { "Trip", "Ticket", "passenger" }).ToList();
                cache.Set("bookings", bookings, TimeSpan.FromMinutes(10));
            }
            return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings Fetched successfully", bookings, true);
        }

        public ResponseModel<List<BookingDTO>> GetBookingsByPassengerId(string passengerid)
        {
            List<BookingDTO> bookingByPassengerId = null!;

            if(cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                bookingByPassengerId = bookings.Where(b => b.passengerId == passengerid).ToList()!;
            else
                bookingByPassengerId = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(b => b.passengerId == passengerid, new string[] { "Trip", "Ticket", "passenger" }).ToList();

            return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings fetched successfully!", bookingByPassengerId);
        }

        public ResponseModel<List<BookingDTO>> GetBookingsByRangeOfDate(DateTime start, DateTime end)
        {
            List<BookingDTO> bookingByRangeOfDate = null!;

            if (cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                bookingByRangeOfDate = bookings.Where(b => b.BookingDate >= start && b.BookingDate <= end).ToList()!;
            else
                bookingByRangeOfDate = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(b => b.BookingDate >= start && b.BookingDate <= end, new string[] { "Trip", "Ticket", "passenger" }).ToList();

            return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings fetched successfully!", bookingByRangeOfDate);
        }

        public ResponseModel<List<BookingDTO>> GetBookingsByTicketId(int ticketid)
        {
            List<BookingDTO> bookingByTicketId = null!;

            if (cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                bookingByTicketId = bookings.Where(b => b.TicketId == ticketid).ToList()!;
            else
                bookingByTicketId = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(b => b.TicketId == ticketid, new string[] { "Trip", "Ticket", "passenger" }).ToList();

            return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings fetched successfully!", bookingByTicketId);
        }

        public ResponseModel<List<BookingDTO>> GetBookingsByTripId(int tripid)
        {
            List<BookingDTO> bookingByTripId = null!;

            if (cache.TryGetValue("bookings", out List<BookingDTO> bookings))
                bookingByTripId = bookings.Where(b => b.TripId == tripid).ToList()!;
            else
                bookingByTripId = unitOfWork.GetRepository<Booking>().FindAll<BookingDTO>(b => b.TripId == tripid, new string[] { "Trip", "Ticket", "passenger" }).ToList();

            return ResponseModelFactory<List<BookingDTO>>.CreateResponse("Bookings fetched successfully!", bookingByTripId);
        }

        public ResponseModel<BookingDTO> UpdateBooking(int bookingid, BookingDTO Updatedbooking)
        {
            var booking = unitOfWork.GetRepository<Booking>().Find(b => b.BookingId == bookingid, new string[] { "Trip", "Ticket", "passenger" });

            if (booking == null)
                return ResponseModelFactory<BookingDTO>.CreateResponse("Booking not found", null!, false);

            try
            {
                booking.TicketId =  Convert.ToInt32(Updatedbooking.TicketId);
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
}
