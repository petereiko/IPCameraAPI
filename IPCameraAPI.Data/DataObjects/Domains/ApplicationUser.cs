using IPCameraAPI.Data.DataObjects.BaseEntityModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPCameraAPI.Data.DataObjects.Domains
{
    public class ApplicationUser : BaseObject
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CameraIpAddress { get; set; }
        public string CameraUsername { get; set; }
        public string CameraPassword { get; set; }
        public int CameraPort { get; set; }
        [ForeignKey("SubscriptionPlanId")]
        public Guid? SubscriptionPlanId { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    }
}
