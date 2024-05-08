using IPCameraAPI.Data.DataObjects.BaseEntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Data.DataObjects.Domains
{
    public class AdminUser:BaseObject
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }
}
