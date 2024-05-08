using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.DTOs
{
    public class SubscriptionPlanDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? Duration { get; set; }
        public decimal? Amount { get; set; }
        public Guid AdminUserId { get; set; }
        public bool IsActive { get; set; }
        public AdminUserDto AdminUser { get; set; }
    }
}
