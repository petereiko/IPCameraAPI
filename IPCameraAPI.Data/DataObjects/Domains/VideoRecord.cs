using IPCameraAPI.Data.DataObjects.BaseEntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Data.DataObjects.Domains
{
    public class VideoRecord:BaseObject
    {
        public string FileName { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
