using IPCameraAPI.Data.DataObjects.BaseEntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Data.DataObjects.Domains
{
    public class SubscriptionPlan:BaseObject
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Amount { get; set; }
        public Guid AdminUserId { get; set; }   
        public virtual AdminUser AdminUser { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
    }
}
