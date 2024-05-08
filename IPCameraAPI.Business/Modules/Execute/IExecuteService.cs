using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Modules.Execute
{
    public interface IExecuteService
    {
        Task<ApiResult<VideoRecordFileResult>> Run(LoginDto request, string directory, string fileName, int duration);
    }
}
