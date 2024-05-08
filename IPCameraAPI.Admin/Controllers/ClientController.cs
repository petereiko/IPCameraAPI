using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IPCameraAPI.Admin.Controllers
{
    public class ClientController : AuthController
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IAdminUserService _adminUserService;
        private readonly IHttpContextAccessor _contextAccessor;
        public ClientController(ILogger<ClientController> logger, IAdminUserService adminUserService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _logger = logger;
            _adminUserService = adminUserService;
            _contextAccessor = contextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _adminUserService.GetAllClientUsers(new Pagination { PageIndex = 1, PageSize = 50, Search = "" });
            return View(result);
        }

        [HttpGet]
        public async Task<PartialViewResult> CreateClient()
        {
            ApplicationUserDto model = new();
            model.SubscriptionPlas = (await _adminUserService.GetAllSubscriptionPlans()).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = x.Duration == 1 ? $"{x.Name} - {x.Amount} for 1 Month" : $"{x.Name} - {x.Amount} for {x.Duration} Months",
                Value = x.Id.ToString()
            }).ToList();
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient(ApplicationUserDto model)
        {
            var result = await _adminUserService.CreateClientUser(model);
            TempData["Message"] = JsonConvert.SerializeObject(result);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<PartialViewResult> EditClient(Guid id)
        {
            var model = await _adminUserService.GetClientUser(id);
            model.SubscriptionPlas = (await _adminUserService.GetAllSubscriptionPlans()).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = x.Duration == 1 ? $"{x.Name} - {x.Amount} for 1 Month" : $"{x.Name} - {x.Amount} for {x.Duration} Months",
                Value = x.Id.ToString()
            }).ToList();
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditClient(ApplicationUserDto model)
        {
            model.ModifiedBy = UserDetail.Id;
            var result = await _adminUserService.UpdateClientUser(model);
            TempData["Message"] = JsonConvert.SerializeObject(result);
            return RedirectToAction("Index");
        }
    }
}
