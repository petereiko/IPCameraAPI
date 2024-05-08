using IPCameraAPI.Business.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IPCameraAPI.Admin.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public AdminUserDto UserDetail { get
            {
                if (_contextAccessor.HttpContext.Session.GetString("user") == null) { return null; }
                return JsonConvert.DeserializeObject<AdminUserDto>(_contextAccessor.HttpContext.Session.GetString("user"));
            }
        }

        //public AdminUserDto UserDetail => _contextAccessor == null? null: JsonConvert.DeserializeObject<AdminUserDto>(_contextAccessor.HttpContext.Session.GetString("user"));
    }
}
