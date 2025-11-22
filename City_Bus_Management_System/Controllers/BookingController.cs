using Service_Layer.Services;

namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        public IBookingService bookingService { get; set; }
        public BookingController(IBookingService _bookingService)
        {
            bookingService = _bookingService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllBookings()
        {
            var result = bookingService.GetBookings();

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{passengerid}")]

        [Authorize(Roles = "Passenger")]
        public IActionResult GetBookingByPassengerId(string passengerid)
        {
            var result = bookingService.GetBookingsByPassengerId(passengerid);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("trip/{tripid}")]
        [Authorize(Roles = "Admin")]

        public IActionResult GetBookingByTripId(int tripid)
        {
            var result = bookingService.GetBookingsByTripId(tripid);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("ticket/{ticketid}")]
        [Authorize(Roles = "Admin")]

        public IActionResult GetBookingByTicketId(int ticketid)
        {
            var result = bookingService.GetBookingsByTicketId(ticketid);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("date")]
        [Authorize(Roles = "Admin")]

        public IActionResult GetBookingByRangeOfDate([FromQuery] DateTime start,[FromQuery] DateTime end)
        {
            var result = bookingService.GetBookingsByRangeOfDate(start, end);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost]
        [Authorize(Roles = "Passenger")]

        public IActionResult BookTicket([FromBody] BookingDTO booking)
        {
            var result = bookingService.BookTicket(booking);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPut("{bookingid}")]
        [Authorize(Roles = "Passenger")]

        public IActionResult UpdateBooking(int bookingid, [FromBody] BookingDTO booking)
        {
            var result = bookingService.UpdateBooking(bookingid, booking);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPatch("cancel/{bookingid}")]
        [Authorize(Roles = "Passenger")]

        public IActionResult CancelBooking(int bookingid)
        {
            var result = bookingService.CancelBooking(bookingid);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("StartStation/{passid}")]
        [Authorize(Roles = "Passenger")]

        public IActionResult GetPassengerStartStationAsync(string passid)
        {
            var result = bookingService.GetPassengerStartStationAsync(passid);

            return Ok(result);
        }
    }
}
