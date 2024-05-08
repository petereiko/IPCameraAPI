using IPCameraAPI.Data.DataObjects.BaseEntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.DTOs
{
    public class AlarmEventDto:BaseObject
    {
        public string MediaFile { get; set; }
    }
}
