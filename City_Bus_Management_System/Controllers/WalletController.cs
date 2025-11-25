using City_Bus_Management_System.DataLayer;
using System.Text.Json;

namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Passenger")]
    public class WalletController : ControllerBase
    {
        public IWalletService walletService { get; set; }
        public ILogger<WalletController> Logger { get; set; }
        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            this.walletService = walletService;
            Logger = logger;
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

        [HttpPost("SendMoney")]
        public async Task<IActionResult> SendMoneyBetweenTwoWallets([FromBody] TransformMoneyDTO transformMoneyDTO)
        {
            var response = await walletService.SendMoneyBetweenTwoWallets(transformMoneyDTO);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymobCallback([FromBody] PaymobCallback payload)
        {
            ResponseModel<object> result = null!;
            try
            {
                Logger.LogInformation("Received Paymob callback: {@Payload}", payload);

                var hmacHeader = Request.Query["hmac"].FirstOrDefault();

                result = await walletService.PaymobCallback(payload, hmacHeader);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error logging Paymob callback");
            }
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
