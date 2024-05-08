using IPCameraAPI.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Modules.Streaming
{
    public interface IStreamingService
    {
        Task<ApiResult<byte[]>> Start();

        Task<ApiResult> Stop();
    }
}
