using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service_Layer.IServices;

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
        [HttpPut("{amount}/{passengerId}")]
        public IActionResult ChargeWallet(double amount, string passengerId)
        {
            var response = walletService.ChargeWallet(amount, passengerId);
            
            return response.IsSuccess ? Ok(response) : BadRequest(response.Message);
        }
    }
}
