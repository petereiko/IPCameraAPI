using IPCameraAPI.Business.Modules.Alarm;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PCameraAPI.ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly IAlarmService _alarmService;
        public AlarmController(IAlarmService alarmService)
        {
            _alarmService = alarmService;
        }

        [HttpGet("activate")]
        public async Task<IActionResult> Activate()
        {
            var result = await _alarmService.Activate();
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("deactivate")]
        public async Task<IActionResult> Deactivate()
        {
            var result = await _alarmService.Deactivate();
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
