using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Modules.Authentication;
using IPCameraAPI.Business.Modules.Record;
using IPCameraAPI.Business.Modules.Streaming;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static IPCameraAPI.Business.CHCNetSDK;

namespace IPCameraAPI.Business.Modules.Execute
{
    public class ExecuteService:IExecuteService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IStreamingService _streamingService;
        private readonly IRecordService _recordService;
        private readonly AppSetting _appSetting;
        //private readonly ICookieStore _cookieStore;

        private bool m_bInitSDK;
        private int m_lUserID;
        private int m_lRealHandle=-1;
        REALDATACALLBACK RealData = null;
        private uint iLastErr;
        private int m_lPlayHandle=-1;
        private bool m_bRecord;

        public ExecuteService(IOptions<AppSetting> options, IAuthenticationService authenticationService, IStreamingService streamingService, IRecordService recordService)
        {
            _appSetting = options.Value;
            _authenticationService = authenticationService;
            _streamingService = streamingService;
            _recordService = recordService;
        }

        public async Task<ApiResult<VideoRecordFileResult>> Run(LoginDto request, string directory, string fileName, int duration)
        {
            ApiResult<VideoRecordFileResult> apiResult = new() { Result = new(), StatusCode = HttpStatusCode.BadRequest };
            //Login to the device
            
            var loginRequest = await Login(request);
            if (loginRequest.StatusCode != HttpStatusCode.OK)
            {
                apiResult.Message = loginRequest.Message;
                return apiResult;
            }
            //Stream Video
            var result = await StartStreaming();
            if(result.StatusCode!=HttpStatusCode.OK)
            {
                await Logout();
                apiResult.Message = result.Message;
                apiResult.StatusCode = result.StatusCode;
                return apiResult;
            }
            //Start Recording

            apiResult = await StartRecording(directory, fileName);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                await Logout();
                return apiResult;
            }
            Thread.Sleep(duration);
            //Stop Recording
            result = await StopRecording();
            if (result.StatusCode != HttpStatusCode.OK)
            {
                await Logout();
                apiResult.Message = result.Message;
                apiResult.StatusCode = result.StatusCode;
                return apiResult;
            }
            //Stop Video Stream
            result = await StopStreaming();
            if (result.StatusCode != HttpStatusCode.OK)
            {
                apiResult.Message = result.Message;
                apiResult.StatusCode = result.StatusCode;
                return apiResult;
            }
            //Logout of the device
            var logoutResult = await Logout();
            if (logoutResult.StatusCode != HttpStatusCode.OK)
            {
                apiResult.Message = result.Message;
                apiResult.StatusCode = result.StatusCode;
                return apiResult;
            }
            result.Message = "Record saved successfully";

            apiResult.StatusCode = HttpStatusCode.OK;
            apiResult.Status = true;
            return apiResult;
        }



        private async Task<ApiResult<LoginResponseDto>> Login(LoginDto model)
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

            //_cookieStore.StoreStringData(Constants.m_lUserID, m_lUserID.ToString());

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

        public async Task<ApiResult> StartStreaming()
        {
            ApiResult result = new();

            if (m_lUserID < 0)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Message = "Please login to the device";
                return result;
            }


            if (m_lRealHandle < 0)
            {
                NET_DVR_PREVIEWINFO lpPreviewInfo = new NET_DVR_PREVIEWINFO();
                //lpPreviewInfo.hPlayWnd = pictureBox1.Handle;//For Windows Application
                lpPreviewInfo.lChannel = 1;//Ô¤teÀÀµÄÉè±¸Í¨µÀ
                lpPreviewInfo.dwStreamType = 0;//ÂëÁ÷ÀàÐÍ£º0-Ö÷ÂëÁ÷£¬1-×ÓÂëÁ÷£¬2-ÂëÁ÷3£¬3-ÂëÁ÷4£¬ÒÔ´ËÀàÍÆ
                lpPreviewInfo.dwLinkMode = 0;//Á¬½Ó·½Ê½£º0- TCP·½Ê½£¬1- UDP·½Ê½£¬2- ¶à²¥·½Ê½£¬3- RTP·½Ê½£¬4-RTP/RTSP£¬5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- ·Ç×èÈûÈ¡Á÷£¬1- ×èÈûÈ¡Á÷
                lpPreviewInfo.dwDisplayBufNum = 1; //²¥·Å¿â²¥·Å»º³åÇø×î´ó»º³åÖ¡Êý
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;


                //if (textBoxID.Text != "")
                //{
                //    lpPreviewInfo.lChannel = -1;
                //    byte[] byStreamID = System.Text.Encoding.Default.GetBytes(textBoxID.Text);
                //    lpPreviewInfo.byStreamID = new byte[32];
                //    byStreamID.CopyTo(lpPreviewInfo.byStreamID, 0);
                //}


                if (RealData == null)
                {
                    RealData = new REALDATACALLBACK(RealDataCallBack);//Ô¤ÀÀÊµÊ±Á÷»Øµ÷º¯Êý
                }

                IntPtr pUser = new IntPtr();//ÓÃ»§Êý¾Ý

                //´ò¿ªÔ¤ÀÀ Start live view 
                var m_lRealHandleResult = NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                m_lRealHandle = m_lRealHandleResult;
                //_cookieStore.StoreStringData(Constants.m_lRealHandle, m_lRealHandle.ToString());

                if (m_lRealHandle < 0)
                    iLastErr = NET_DVR_GetLastError();

                result.Message = !(m_lRealHandle < 0) ? "Streaming started successfully" : "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr;
                result.StatusCode = !(m_lRealHandle < 0) ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
                m_lPlayHandle = !(m_lRealHandle < 0) ? 0 : -1;
                result.Status = !(m_lRealHandle < 0);
                return await Task.FromResult(result);
            }

            result.Message = "Currently streaming";
            result.StatusCode = HttpStatusCode.BadRequest;
            result.Status = false;
            return await Task.FromResult(result);
        }

        private void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            if (dwBufSize > 0)
            {
                byte[] sData = new byte[dwBufSize];
                Marshal.Copy(pBuffer, sData, 0, (Int32)dwBufSize);

                string str = @"C:\IPCamera\Videos\" + DateTime.Now.Ticks + ".ps";
                //string str = "ÊµÊ±Á÷Êý¾Ý.ps";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)dwBufSize;
                fs.Write(sData, 0, iLen);
                fs.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory">Must have a trailing forward slash /</param>
        /// <param name="fileName">Name of file must have the .mp4 extension</param>
        /// <returns></returns>
        private async Task<ApiResult<VideoRecordFileResult>> StartRecording(string directory, string fileName)
        {
            ApiResult<VideoRecordFileResult> result = new() { Result = new(), StatusCode = HttpStatusCode.BadRequest };
            //Â¼Ïñ±£´æÂ·¾¶ºÍÎÄ¼þÃû the path and file name to save
            if (!fileName.EndsWith(".mp4")) fileName += ".mp4";
            if (!directory.EndsWith("/")) directory += "/";
            string sVideoFileName = Path.Combine(directory, fileName);

            if (m_bRecord == false)
            {
                //Ç¿ÖÆIÖ¡ Make a I frame
                int lChannel = Int16.Parse("1"); //Í¨µÀºÅ Channel number
                NET_DVR_MakeKeyFrame(m_lUserID, lChannel);

                bool recordingStatus = NET_DVR_SaveRealData(m_lRealHandle, sVideoFileName);

                if (!recordingStatus)
                    iLastErr = NET_DVR_GetLastError();
                //¿ªÊ¼Â¼Ïñ Start recording

                result.Message = recordingStatus ? "Recording started" : "NET_DVR_SaveRealData failed, error code= " + iLastErr;
                result.Status = recordingStatus;
                result.StatusCode = recordingStatus ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
                m_bRecord = recordingStatus;
                result.Result.MediaName = fileName;
                return await Task.FromResult(result);

            }
            else
            {
                result.Message = "Record already playing in the background";
                result.Status = false;
                result.StatusCode = HttpStatusCode.BadRequest;
                m_bRecord = false;
                return await Task.FromResult(result);
            }
        }

        private async Task<ApiResult> StopRecording()
        {
            ApiResult result = new();
            if (m_bRecord)
            {
                bool isStopped = NET_DVR_StopSaveRealData(m_lRealHandle);
                if (!isStopped)
                    iLastErr = NET_DVR_GetLastError();

                result.Message = isStopped ? "Record stopped successfully" : "NET_DVR_StopSaveRealData failed, error code= " + iLastErr;
                result.Status = isStopped;
                result.StatusCode = isStopped ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
                return await Task.FromResult(result);
            }
            result.Message = "No record current playing";
            result.Status = false;
            result.StatusCode = HttpStatusCode.BadRequest;
            return await Task.FromResult(result);

        }

        private async Task<ApiResult> StopStreaming()
        {
            ApiResult result = new();
            if (m_lRealHandle >= 0)
            {
                bool isStopped = NET_DVR_StopRealPlay(m_lRealHandle);
                //Í£Ö¹Ô¤ÀÀ Stop live view 
                if (!isStopped)
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                result.Message = isStopped ? "Stream stopped successfully" : "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                result.Status = isStopped;
                result.StatusCode = isStopped ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
                m_lRealHandle = isStopped ? -1 : 0;
            }
            else
            {
                result.Message = "No Stream currently on";
            }
            return await Task.FromResult(result);
        }

        private async Task<ApiResult<LogoutResponseDto>> Logout()
        {
            ApiResult<LogoutResponseDto> result = new() { Result = new() };

            await StopRecording(); //_recordService.Stop();

            await StopStreaming(); //_streamingService.Stop();

            if (!NET_DVR_Logout(m_lUserID))
            {
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Status = false;
                result.Message = "Could not logout";
            }
            else
            {
                //_cookieStore.RemoveData(Constants.m_lUserID);
                //_cookieStore.RemoveData(Constants.m_lRealHandle);
                //_cookieStore.RemoveData(Constants.m_bRecord);
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Status = true;
                result.Message = "Logout successful";
            }
            return await Task.FromResult(result);
        }

    }
}
