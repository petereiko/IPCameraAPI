using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Modules.Execute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPCameraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecuteController : ControllerBase
    {
        private readonly IExecuteService _executeService;
        public ExecuteController(IExecuteService executeService)
        {
            _executeService = executeService;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run([FromBody] LoginDto request)
        {
            var result = await _executeService.Run(request);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
