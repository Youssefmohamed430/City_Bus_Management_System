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
        [HttpPut("{amount}/{passengerid}")]
        public async Task<IActionResult> ChargeWallet(double amount,string passengerid)
        {
            try
            {
                var iframeUrl = await walletService.ChargeWallet(amount,passengerid);

                return Ok(new { iframeUrl = iframeUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("callback")]
        public async Task<IActionResult> PaymobCallback([FromBody] PaymobCallback payload,string passengerid)
        {
            string hmacHeader = Request.Headers["hmac"].FirstOrDefault()!;

            var result = await walletService.PaymobCallback(payload, hmacHeader,passengerid);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
