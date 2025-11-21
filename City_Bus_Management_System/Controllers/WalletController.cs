namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        public IWalletService walletService { get; set; }
        public WalletController(IWalletService walletService)
        {
            this.walletService = walletService;
        }
        [HttpGet("{Id}")]
        public IActionResult GetWalletByPassengerId(string Id)
        {
            var response = walletService.GetWalletByPassengerId(Id);

            return response.IsSuccess ? Ok(response) : BadRequest(response.Message);
        }
        [HttpPut("{amount}")]
        public IActionResult ChargeWallet(double amount)
        {
            var response = walletService.ChargeWallet(amount);
            
            return Ok(response);
        }
        [HttpPost("callback/{passengerid}")]
        public async Task<IActionResult> PaymobCallback([FromBody] PaymobCallback payload,string passengerid)
        {
            string hmacHeader = Request.Headers["hmac"].FirstOrDefault()!;

            var result = await walletService.PaymobCallback(payload, hmacHeader,passengerid);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
