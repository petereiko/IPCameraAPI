using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Interfaces;
using IPCameraAPI.Business.Modules.Authentication;
using IPCameraAPI.Data;
using IPCameraAPI.Data.DataObjects.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using static IPCameraAPI.Business.CHCNetSDK;

namespace IPCameraAPI.Business.Modules.Alarm
{
    public class AlarmService : IAlarmService
    {
        private int handle = -1;
        private int m_lUserID = -1;
        private MSGCallBack_V31 m_falarmData_V31 = null;

        private readonly ILogger<AlarmService> _logger;
        private readonly ICookieStore _cookieStore;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        //private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly LoginDto _loginDto;
        private readonly AppSetting _appSetting;
        DateTime lastDate = DateTime.Now;
        public AlarmService(IOptions<AppSetting> options, ILogger<AlarmService> logger, ICookieStore cookieStore, INotificationService notificationService, ApplicationDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _cookieStore = cookieStore;
            int.TryParse(cookieStore.GetStringData(Constants.m_lUserID), out m_lUserID);
            _loginDto = JsonConvert.DeserializeObject<LoginDto>(cookieStore.GetStringData(Constants.UserData));
            lastDate = DateTime.Now;
            _notificationService = notificationService;
            _context = context;
            _appSetting = options.Value;
            _configuration = configuration;
        }

        public void Init()
        {
            byte[] strIP = new byte[16 * 16];
            uint dwValidNum = 0;
            bool bEnableBind = false;
            //Obtain local PC network card IP information
            if (NET_DVR_GetLocalIP(strIP, ref dwValidNum, ref bEnableBind))
            {
                if (dwValidNum > 0)
                {
                    //Take the IP address of the first network card as the default listening port
                    //textBoxListenIP.Text = System.Text.Encoding.UTF8.GetString(strIP, 0, 16);
                    NET_DVR_SetValidIP(0, true); //Bind the first network card
                }
            }
            //To save the SDK log
            NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            handle = -1;
            //Set alarm callback function
            if (m_falarmData_V31 == null)
            {
                m_falarmData_V31 = new MSGCallBack_V31(MsgCallback_V31);
            }
            NET_DVR_SetDVRMessageCallBack_V31(m_falarmData_V31, IntPtr.Zero);
        }

        //string strErr;
        //private Int32[] m_lAlarmHandle = new Int32[200];
        //int iDeviceNumber = 1;
        uint iLastErr;
        int alarmActivationStatus = -1;


        public async Task<ApiResult> Activate()
        {
            ApiResult result = new() { StatusCode = System.Net.HttpStatusCode.BadRequest };
            Init();
            NET_DVR_SETUPALARM_PARAM struAlarmParam = new();
            struAlarmParam.dwSize = (uint)Marshal.SizeOf(struAlarmParam);
            struAlarmParam.byLevel = 1; //0-Level 1 arming, 1- Level 2 arming
            struAlarmParam.byAlarmInfoType = 1;//Intelligent transportation equipment is effective, new alarm message type
            struAlarmParam.byFaceAlarmDetection = 1;//1-Face detection

            var userData = _cookieStore.GetStringData(Constants.m_lUserID);
            m_lUserID = Convert.ToInt32(userData);
            if (m_lUserID < 0)
            {
                result.Message = "Kindly login";
                return result;
            }
            alarmActivationStatus = NET_DVR_SetupAlarmChan_V41(m_lUserID, ref struAlarmParam);
            if (alarmActivationStatus < 0)
            {
                iLastErr = NET_DVR_GetLastError();
                result.Message = "Failed to arm, Error code:" + iLastErr;
                return result;
            }

            _cookieStore.StoreStringData(Constants.alarmActivationStatus, alarmActivationStatus.ToString());

            result.Message = "Arm successfully";
            result.StatusCode = System.Net.HttpStatusCode.OK;
            result.Status = true;
            return await Task.FromResult(result);

            //for (int i = 0; i < iDeviceNumber; i++)
            //{
            //    m_lAlarmHandle[m_lUserID] = NET_DVR_SetupAlarmChan_V41(m_lUserID, ref struAlarmParam);
            //    if (m_lAlarmHandle[m_lUserID] < 0)
            //    {
            //        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
            //        strErr = "Failed to arm, Error code:" + iLastErr;
            //        MessageBox.Show(strErr);
            //        //listViewDevice.Items[i].SubItems[2].Text = strErr;
            //    }
            //    else
            //    {
            //        MessageBox.Show("Arm successfully");
            //    }
            //}
        }

        public async Task<ApiResult> Deactivate()
        {
            ApiResult result = new() { StatusCode = System.Net.HttpStatusCode.BadRequest };
            try
            {
                var userData = _cookieStore.GetStringData(Constants.m_lUserID);
                m_lUserID = Convert.ToInt32(userData);
                if (m_lUserID < 0)
                {
                    result.Message = "Kindly login";
                    return result;
                }

                var alarmStatus = _cookieStore.GetStringData(Constants.alarmActivationStatus);
                alarmActivationStatus = Convert.ToInt32(alarmStatus);
                if (alarmActivationStatus >= 0)
                {
                    if (!NET_DVR_CloseAlarmChan_V30(alarmActivationStatus))
                    {
                        iLastErr = NET_DVR_GetLastError();
                        result.Message = "Failed to disarm, Error code:" + iLastErr; //撤防失败，输出错误号
                    }
                    else
                    {

                        //listViewDevice.Items[i].SubItems[2].Text = "Disarmed";
                        alarmActivationStatus = -1;
                        _cookieStore.StoreStringData(Constants.alarmActivationStatus, alarmActivationStatus.ToString());
                        result.Message = "Alarm disarmed";
                        result.StatusCode = System.Net.HttpStatusCode.OK;
                        result.Status = true;
                    }
                }
                else
                {
                    alarmActivationStatus = -1;
                    _cookieStore.StoreStringData(Constants.alarmActivationStatus, alarmActivationStatus.ToString());
                    result.Message = "Alarm already disarmed";
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                result.Message = "An error occurred";
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            }
            return await Task.FromResult(result);
        }

        private bool MsgCallback_V31(int lCommand, ref NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {

            //Pass lCommandto determine the type of alarm message received，distinct lCommand Corresponds to a different one pAlarmInfo content

            AlarmMessageHandle(lCommand, ref pAlarmer, pAlarmInfo, dwBufLen, pUser);



            return true; //The callback function needs to have a return，Indicates that the data is received normally

        }

        private void ProcessCommAlarm(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        
        int counter = 0;
        private void ProcessCommAlarm_V30(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser)
        {
            if (counter == 0)
            {
                ExecuteAction();
            }
            else
            {
                if ((DateTime.Now - lastDate).Minutes > 1)
                {
                    ExecuteAction();
                }
            }
        }


        private ApiResult<VideoRecordFileResult> ExecuteAction(string endpoint = "https://localhost:7226/api/execute/run")
        {
            ApiResult<VideoRecordFileResult> apiResult = new() { StatusCode = System.Net.HttpStatusCode.BadRequest, Result = new() };
            try
            {
                counter++;
                lastDate = DateTime.Now;
                //var userData = _cookieStore.GetStringData(Constants.UserData);
                if (_loginDto == null)
                {
                    apiResult.Message = "Unauthenticated User";
                    return apiResult;
                }

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                //request.Headers.Add("Cookie", "m_bRecord=False; m_lRealHandle=-1; m_lUserID=1");
                var content = new StringContent(JsonConvert.SerializeObject(_loginDto), null, "application/json");
                request.Content = content;
                var response = client.SendAsync(request).GetAwaiter().GetResult();
                apiResult = JsonConvert.DeserializeObject<ApiResult<VideoRecordFileResult>>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if(apiResult.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    var connectionString = _configuration.GetConnectionString("DefaultConnection");
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlServer(connectionString);
                    ApplicationDbContext context = new ApplicationDbContext(optionsBuilder.Options);

                    AlarmEvent alarm = new()
                    {
                        CreatedDate = DateTime.Now,
                        IpAddress = _loginDto.IPAddress,
                        IsActive = true,
                        IsDeleted = false,
                        MediaFile = apiResult.Result.MediaName,
                        Username = _loginDto.Username
                    };
                    context.AlarmEvents.Add(alarm);
                    context.SaveChanges();
                    
                    var user = context.ApplicationUsers.AsNoTracking().Where(x => x.CameraIpAddress == _loginDto.IPAddress && x.CameraUsername == _loginDto.Username)?.FirstOrDefault();
                    if (user != null)
                    {
                        string message = "Dear User, an event has been triggered by your Camera and we have captured it. Kindly check your registered email for the video file";
                        _notificationService.SendSms(message, user.Phone).GetAwaiter().GetResult();
                        //var alarmEvent = context.AlarmEvents.AsNoTracking().Where(x => x.IpAddress == _loginDto.IPAddress && x.Username == _loginDto.Username).OrderByDescending(x=>x.CreatedDate).FirstOrDefault();
                        //if (alarmEvent!=null)
                        //{
                            string filePath = $"{_appSetting.VideoPath}{apiResult.Result.MediaName}";
                            byte[] attachment = File.ReadAllBytes(filePath);
                        List<string> paths = new List<string> { filePath };
                            _notificationService.SendEmailWithAttachment(message, paths, user.Email, "CCTV Video File").GetAwaiter().GetResult();
                        ///}
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                apiResult.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                apiResult.Message = ex.Message;
            }
            return apiResult;
        }

        private void ProcessCommAlarm_RULE(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_Plate(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_ITSPlate(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_PDC(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_PARK(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_VQD(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_FaceSnap(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_FaceMatch(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_FaceDetect(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_CIDAlarm(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_AcsAlarm(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void ProcessCommAlarm_IDInfoAlarm(ref NET_DVR_ALARMER pAlarmer, nint pAlarmInfo, uint dwBufLen, nint pUser) { }

        private void AlarmMessageHandle(int lCommand, ref NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            switch (lCommand)
            {
                case COMM_ALARM: //(DS-8000老设备)移动侦测、视频丢失、遮挡、IO信号量等报警信息
                    ProcessCommAlarm(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;

                case COMM_ALARM_V30://移动侦测、视频丢失、遮挡、IO信号量等报警信息

                    ProcessCommAlarm_V30(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_ALARM_RULE://进出区域、入侵、徘徊、人员聚集等异常行为检测报警信息

                    ProcessCommAlarm_RULE(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_UPLOAD_PLATE_RESULT://交通抓拍结果上传(老报警信息类型)

                    ProcessCommAlarm_Plate(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_ITS_PLATE_RESULT://交通抓拍结果上传(新报警信息类型)

                    ProcessCommAlarm_ITSPlate(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_ALARM_PDC://客流量统计报警信息

                    ProcessCommAlarm_PDC(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_ITS_PARK_VEHICLE://客流量统计报警信息

                    ProcessCommAlarm_PARK(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_DIAGNOSIS_UPLOAD://VQD报警信息

                    ProcessCommAlarm_VQD(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_UPLOAD_FACESNAP_RESULT://人脸抓拍结果信息

                    ProcessCommAlarm_FaceSnap(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_SNAP_MATCH_ALARM://人脸比对结果信息

                    ProcessCommAlarm_FaceMatch(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_ALARM_FACE_DETECTION://人脸侦测报警信息

                    ProcessCommAlarm_FaceDetect(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_ALARMHOST_CID_ALARM://报警主机CID报警上传

                    ProcessCommAlarm_CIDAlarm(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_ALARM_ACS://门禁主机报警上传

                    ProcessCommAlarm_AcsAlarm(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                case COMM_ID_INFO_ALARM://身份证刷卡信息上传

                    ProcessCommAlarm_IDInfoAlarm(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

                    break;

                default:
                    {
                        //string stringAlarm = "upload alarm，alarm message type：" + lCommand;
                        //if (InvokeRequired)
                        //{
                        //    object[] paras = new object[3];
                        //    paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                        //    paras[1] = _ipAddress;
                        //    paras[2] = stringAlarm;
                        //    listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
                        //}
                        //else
                        //{
                        //    //The main thread that created the control directly updates the information list

                        //    UpdateClientList(DateTime.Now.ToString(), _ipAddress, stringAlarm);

                        //}
                        //Do your update here


                    }

                    break;

            }

        }

    }
}
