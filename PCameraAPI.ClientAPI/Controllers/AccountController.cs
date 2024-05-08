using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Interfaces;
using IPCameraAPI.Business.Modules.Authentication;
using IPCameraAPI.Business.Validations;
using IPCameraAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PCameraAPI.ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ApplicationDbContext _context;
        public AccountController(IAuthenticationService authenticationService, ApplicationDbContext context)
        {
            _authenticationService = authenticationService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto request)
        {
            var result = await _authenticationService.Login(request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authenticationService.Logout();
            return StatusCode((int)result.StatusCode, result);
        }

        
    }
}
