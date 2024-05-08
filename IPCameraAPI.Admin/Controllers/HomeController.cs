using IPCameraAPI.Admin.Models;
using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Printing;

namespace IPCameraAPI.Admin.Controllers
{
    public class HomeController : AuthController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdminUserService _adminUserService;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(ILogger<HomeController> logger, IAdminUserService adminUserService, IHttpContextAccessor contextAccessor):base(contextAccessor)
        {
            _logger = logger;
            _adminUserService = adminUserService;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            if (UserDetail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Plan()
        {
            return View(await _adminUserService.GetAllSubscriptionPlans());
        }

        [HttpGet]
        public IActionResult CreatePlan()
        {
            return PartialView(new SubscriptionPlanDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlan(SubscriptionPlanDto model)
        {
            model.AdminUserId = UserDetail.Id;
            var result = await _adminUserService.CreateSubscriptionPlan(model);
            TempData["Message"] = JsonConvert.SerializeObject(result);
            return RedirectToAction("Plan", "Home");
        }

        public async Task<IActionResult> EditPlan(Guid id)
        {
            return PartialView(await _adminUserService.GetSubscriptionPlan(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditPlan(SubscriptionPlanDto model)
        {
            model.AdminUserId = UserDetail.Id;
            var result = await _adminUserService.UpdateSubscriptionPlan(model);
            TempData["Message"] = JsonConvert.SerializeObject(result);
            return RedirectToAction("Plan", "Home");
        }

        

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
