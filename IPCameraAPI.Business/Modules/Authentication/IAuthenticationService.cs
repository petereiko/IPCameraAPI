using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Modules.Authentication
{
    public interface IAuthenticationService
    {
        Task<ApiResult<LoginResponseDto>> Login(LoginDto model);

        Task<ApiResult<LogoutResponseDto>> Logout();

        
    }
}
