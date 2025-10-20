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
        public IActionResult GetAllTickets()
        {
            var result = ticketService.GetAllTickets();

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("{numOfStations}")]
        public IActionResult GetTicketsByNumberOfStations(int numOfStations)
        {
            var result = ticketService.GetTicketByNumberOfStations(numOfStations);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("type/{numOfStations}/{busType}")]
        public IActionResult GetTicketsByBusTypeAndNumberOfStations(int numOfStations, string busType)
        {
            var result = ticketService.GetTicketsByBusTypeAndNumberOfStations(numOfStations, busType);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("price/{numOfStations}/{minPrice}/{maxPrice}")]
        public IActionResult GetTicketsByNumberOfStationsAndRangeOfPrice(int numOfStations, double minPrice, double maxPrice)
        {
            var result = ticketService.GetTicketByNumberOfStationsAndRangeOfPrice(numOfStations, minPrice, maxPrice);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpGet("single/{numOfStations}/{busType}")]
        public IActionResult GetSingleTicketByBusTypeAndNumberOfStations(int numOfStations, string busType)
        {
            var result = ticketService.GetSingleTicketByBusTypeAndNumberOfStations(numOfStations, busType);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPost]
        public IActionResult AddTicket([FromBody] TicketDTO ticketDTO)
        {
            var result = ticketService.AddTicket(ticketDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateTicket(int id, [FromBody] TicketDTO updatedTicket)
        {
            var result = ticketService.UpdateTicket(id, updatedTicket);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int id)
        {
            var result = ticketService.DeleteTicket(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
