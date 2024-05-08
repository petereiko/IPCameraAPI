using IPCameraAPI.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Modules.Alarm
{
    public interface IAlarmService
    {
        void Init();
        Task<ApiResult> Activate();
        Task<ApiResult> Deactivate();
    }
}
