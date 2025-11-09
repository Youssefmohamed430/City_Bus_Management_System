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
        public IActionResult GetAllBookings()
        {
            var result = bookingService.GetBookings();

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{passengerid}")]
        public IActionResult GetBookingByPassengerId(string passengerid)
        {
            var result = bookingService.GetBookingsByPassengerId(passengerid);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("trip/{tripid}")]
        public IActionResult GetBookingByTripId(int tripid)
        {
            var result = bookingService.GetBookingsByTripId(tripid);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("ticket/{ticketid}")]
        public IActionResult GetBookingByTicketId(int ticketid)
        {
            var result = bookingService.GetBookingsByTicketId(ticketid);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("date")]
        public IActionResult GetBookingByRangeOfDate([FromQuery] DateTime start,[FromQuery] DateTime end)
        {
            var result = bookingService.GetBookingsByRangeOfDate(start, end);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost]
        public IActionResult BookTicket([FromBody] BookingDTO booking)
        {
            var result = bookingService.BookTicket(booking);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPut("{bookingid}")]
        public IActionResult UpdateBooking(int bookingid, [FromBody] BookingDTO booking)
        {
            var result = bookingService.UpdateBooking(bookingid, booking);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPatch("{bookingid}")]
        public IActionResult CancelBooking(int bookingid)
        {
            var result = bookingService.CancelBooking(bookingid);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("StartStation/{passid}")]
        public IActionResult GetPassengerStartStationAsync(string passid)
        {
            var result = bookingService.GetPassengerStartStationAsync(passid);

            return Ok(result);
        }
    }
}
