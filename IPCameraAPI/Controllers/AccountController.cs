using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Modules.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPCameraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        public AccountController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.Login(model);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.Logout();
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
