using IPCameraAPI.Business.Modules.Streaming;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPCameraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly IStreamingService _streamingService;
        public StreamController(IStreamingService streamingService)
        {
            _streamingService = streamingService;
        }

        [HttpGet("Start")]
        public async Task<IActionResult> Start()
        {
            var result = await _streamingService.Start();
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("Stop")]
        public async Task<IActionResult> Stop()
        {
            var result = await _streamingService.Stop();
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
