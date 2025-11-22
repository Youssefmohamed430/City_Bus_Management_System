namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        public ITicketService ticketService { get; set; }
        public TicketController(ITicketService _ticketService)
        {
            ticketService = _ticketService;
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAllTickets()
        {
            var result = ticketService.GetAllTickets();

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("{numOfStations}")]
        [Authorize]
        public IActionResult GetTicketsByNumberOfStations(int numOfStations)
        {
            var result = ticketService.GetTicketByNumberOfStations(numOfStations);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("type/{numOfStations}/{busType}")]
        [Authorize]
        public IActionResult GetTicketsByBusTypeAndNumberOfStations(int numOfStations, string busType)
        {
            var result = ticketService.GetTicketsByBusTypeAndNumberOfStations(numOfStations, busType);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("price/{numOfStations}/{minPrice}/{maxPrice}")]
        [Authorize]
        public IActionResult GetTicketsByNumberOfStationsAndRangeOfPrice(int numOfStations, double minPrice, double maxPrice)
        {
            var result = ticketService.GetTicketByNumberOfStationsAndRangeOfPrice(numOfStations, minPrice, maxPrice);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("single/{numOfStations}/{busType}")]
        [Authorize]
        public IActionResult GetSingleTicketByBusTypeAndNumberOfStations(int numOfStations, string busType)
        {
            var result = ticketService.GetSingleTicketByBusTypeAndNumberOfStations(numOfStations, busType);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddTicket([FromBody] TicketDTO ticketDTO)
        {
            var result = ticketService.AddTicket(ticketDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateTicket(int id, [FromBody] TicketDTO updatedTicket)
        {
            var result = ticketService.UpdateTicket(id, updatedTicket);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int id)
        {
            var result = ticketService.DeleteTicket(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
