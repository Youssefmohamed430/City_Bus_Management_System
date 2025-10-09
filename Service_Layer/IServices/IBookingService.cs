using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Entities;
using Data_Access_Layer.DataLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.IServices
{
    public interface IBookingService
    {
        ResponseModel<List<BookingDTO>> GetBookings();
        ResponseModel<List<BookingDTO>> GetBookingsByPassengerId(string passengerid);
        ResponseModel<List<BookingDTO>> GetBookingsByTripId(int tripid);
        ResponseModel<List<BookingDTO>> GetBookingsByTicketId(int ticketid);
        ResponseModel<List<BookingDTO>> GetBookingsByRangeOfDate(DateTime start,DateTime end);
        ResponseModel<BookingDTO> BookTicket(BookingDTO booking);
        ResponseModel<BookingDTO> UpdateBooking(int bookingid,BookingDTO booking);
        ResponseModel<BookingDTO> CancelBooking(int bookingid);
    }
}
