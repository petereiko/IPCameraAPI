using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Modules.Record;
using IPCameraAPI.Business.Modules.Streaming;
using IPCameraAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Modules.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private bool m_bInitSDK = false;
        private int m_lUserID;
        private readonly ICookieStore _cookieStore;
        private readonly IStreamingService _streamingService;
        private readonly IRecordService _recordService;
        public AuthenticationService(ICookieStore cookieStore, IStreamingService streamingService, IRecordService recordService)
        {
            _cookieStore = cookieStore;
            _streamingService = streamingService;
            _recordService = recordService;
        }

        public async Task<ApiResult<LoginResponseDto>> Login(LoginDto model)
        {
            ApiResult<LoginResponseDto> result = new() { Result = new() };

            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (!m_bInitSDK)
            {
                result.Status = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Message = "Service Error";
                return await Task.FromResult(result);
            }

            //Login the device
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(model.IPAddress, model.Port, model.Username, model.Password, ref DeviceInfo);

            _cookieStore.StoreStringData(Constants.m_lUserID, m_lUserID.ToString());
            _cookieStore.StoreStringData(Constants.UserData, JsonConvert.SerializeObject(model));

            result.Result.m_lUserID = m_lUserID;
            if (m_lUserID < 0)
            {
                result.Message = "Error on Logon";
                result.Status = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            else
            {
                result.Message = "Login Success!";
                result.Status = true;
                result.StatusCode = System.Net.HttpStatusCode.OK;
            }
            return await Task.FromResult(result);
        }

        

        public async Task<ApiResult<LogoutResponseDto>> Logout()
        {
            ApiResult<LogoutResponseDto> result = new() { Result = new() };

            await _recordService.Stop();

            await _streamingService.Stop();

            if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
            {
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Status = false;
                result.Message = "Could not logout";
            }
            else
            {
                _cookieStore.RemoveData(Constants.m_lUserID);
                _cookieStore.RemoveData(Constants.m_lRealHandle);
                _cookieStore.RemoveData(Constants.m_bRecord);
                _cookieStore.RemoveData(Constants.alarmActivationStatus);
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Status = true;
                result.Message = "Logout successful";
            }
            return await Task.FromResult(result);
        }


    }
}
