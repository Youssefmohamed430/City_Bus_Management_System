using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace City_Bus_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        public INotificationService notificationService { get; set; }
        public NotificationController(INotificationService _notificationService)
        {
            notificationService = _notificationService;
        }
        [HttpGet("{Id}")]
        public IActionResult GetNotifsById(string Id)
        {
            var result = notificationService.GetNotificationById(Id);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
