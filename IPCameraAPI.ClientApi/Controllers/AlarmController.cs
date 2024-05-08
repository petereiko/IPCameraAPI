using IPCameraAPI.Business.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPCameraAPI.ClientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Activate([FromBody]ActivateRequest request)
        {

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate([FromBody] DeactivateRequest request)
        {

            return Ok();
        }
    }
}
