using IPCameraAPI.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Modules.Record
{
    public interface IRecordService
    {
        Task<ApiResult> Start();

        Task<ApiResult> Stop();
    }
}
