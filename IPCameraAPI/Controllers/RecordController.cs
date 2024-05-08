using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Modules.Authentication;
using IPCameraAPI.Business.Modules.Record;
using IPCameraAPI.Business.Modules.Streaming;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPCameraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private readonly IRecordService _recordService;

        public RecordController(IRecordService recordService)
        {
            _recordService = recordService;
        }

        [HttpGet("Start")]
        public async Task<IActionResult> Start()
        {
            var result = await _recordService.Start();
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpGet("Stop")]
        public async Task<IActionResult> Stop()
        {
            var result = await _recordService.Stop();
            return StatusCode((int)result.StatusCode, result);
        }


        
    }
}
