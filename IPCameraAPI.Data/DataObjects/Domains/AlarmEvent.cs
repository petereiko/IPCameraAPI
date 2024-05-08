using IPCameraAPI.Data.DataObjects.BaseEntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Data.DataObjects.Domains
{
    public class AlarmEvent : BaseObject
    {
        public string MediaFile { get; set; }
        public string IpAddress { get; set; }
        public string Username { get; set; }
    }
}
