using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.DTOs
{
    public class ApplicationUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CameraIpAddress { get; set; }
        public string CameraUsername { get; set; }
        public string CameraPassword { get; set; }
        public int CameraPort { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        public Guid ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public SubscriptionPlanDto SubscriptionPlan { get; set; }
        public List<SelectListItem> SubscriptionPlas { get; set; } = new List<SelectListItem>();
    }
}
