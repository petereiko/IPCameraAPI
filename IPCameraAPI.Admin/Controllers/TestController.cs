using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPCameraAPI.Admin.Controllers
{
    public class TestController : AuthController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IClientRecordService _clientRecordService;

        public TestController(IHttpContextAccessor contextAccessor, IClientRecordService clientRecordService) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _clientRecordService = clientRecordService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Record(RecordDto model)
        {
            await _clientRecordService.Execute(model.Folder, model.VideoDuration);
            return View();
        }
    }
}
