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
        StationDTO GetPassengerStartStationAsync(string passengerId);
    }
}
