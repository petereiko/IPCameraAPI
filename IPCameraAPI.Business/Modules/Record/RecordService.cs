using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.Modules.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static IPCameraAPI.Business.CHCNetSDK;

namespace IPCameraAPI.Business.Modules.Record
{
    public class RecordService:IRecordService
    {
        ICookieStore _cookieStore;
        public RecordService(ICookieStore cookieStore)
        {
            _cookieStore = cookieStore;
        }

        private int m_lUserID;
        private int m_lRealHandle;
        private int m_lPlayHandle;
        private bool m_bRecord;
        private uint iLastErr;
        public async Task<ApiResult> Start()
        {
            ApiResult result = new();
            //Â¼Ïñ±£´æÂ·¾¶ºÍÎÄ¼þÃû the path and file name to save
            string sVideoFileName = @"C:\Videos\" + DateTime.Now.Ticks.ToString() + ".mp4";
            //sVideoFileName = "Record_test.mp4";

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
                _cookieStore.StoreBooleanData(Constants.m_bRecord, m_bRecord);
                return await Task.FromResult(result);

            }
            else
            {
                result.Message = "Record already playing in the background";
                result.Status = false;
                result.StatusCode = HttpStatusCode.BadRequest;
                _cookieStore.StoreBooleanData(Constants.m_bRecord, false);
                return await Task.FromResult(result);
            }
        }

        public async Task<ApiResult> Stop()
        {
            ApiResult result = new();
            _cookieStore.StoreStringData(Constants.m_bRecord, m_bRecord.ToString());
            m_bRecord = _cookieStore.GetBooleanData(Constants.m_bRecord);
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
    }
}
