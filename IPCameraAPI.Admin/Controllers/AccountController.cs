using Azure.Core;
using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Implementations;
using IPCameraAPI.Business.Interfaces;
using IPCameraAPI.Business.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace IPCameraAPI.Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAdminUserService _adminUserService;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountController(IAdminUserService adminUserService, IHttpContextAccessor contextAccessor)
        {
            _adminUserService = adminUserService;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new AdminLoginDto());
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginDto model)
        {
            var result = await _adminUserService.AdminLogin(model);
            if (result.Status)
            {
                _contextAccessor.HttpContext.Session.SetString("user", JsonConvert.SerializeObject(result.Result));
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View(new AdminUserDto());
        }

        [HttpPost]
        public async Task<IActionResult> Register(AdminUserDto model)
        {
            MessageResponse result = await _adminUserService.CreateAdminUser(model);
            if (result.Status)
                return RedirectToAction("Index", "Home");
            return View(model);
        }

        [HttpGet]
        public IActionResult Clients()
        {
            return View();
        }

        


        public IActionResult CreateClientUser()
        {
            return PartialView(new ApplicationUserDto());
        }

    }
}
