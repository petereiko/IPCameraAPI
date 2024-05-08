using IPCameraAPI.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPCameraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("SendEmail")]
        public async Task<IActionResult> SendEmail()
        {
            List<string> path = new List<string> { "C:\\Videos\\638481691376945084.mp4" };
            await _notificationService.SendEmailWithAttachment("Test Message", path, "peterayebhere@gmail.com", "Motion Detected");
            return Ok("sent");
        }
    }
}
