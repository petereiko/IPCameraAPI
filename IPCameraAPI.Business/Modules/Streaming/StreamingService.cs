using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.Modules.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static IPCameraAPI.Business.CHCNetSDK;

namespace IPCameraAPI.Business.Modules.Streaming
{
    public class StreamingService:IStreamingService
    {
        private readonly ICookieStore _cookieStore;
        private int m_lUserID;
        private int m_lRealHandle;
        private int m_lPlayHandle;
        private bool m_bRecord;
       
        public uint iLastErr;
        public StreamingService(ICookieStore cookieStore)
        {
            _cookieStore = cookieStore;
        }

        REALDATACALLBACK RealData = null;

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
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


        public async Task<ApiResult<byte[]>> Start()
        {
            ApiResult<byte[]> result = new() { StatusCode = HttpStatusCode.BadRequest };

            if (Convert.ToInt32(_cookieStore.GetStringData(Constants.m_lUserID)) < 0)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Message = "Please login to the device";
                return result;
            }

            m_lUserID = Convert.ToInt32(_cookieStore.GetStringData(Constants.m_lUserID));

            if (Convert.ToInt32(_cookieStore.GetStringData(Constants.m_lRealHandle)) < 0)
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
               var m_lRealHandle = NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);

                _cookieStore.StoreStringData(Constants.m_lRealHandle, m_lRealHandle.ToString());

                if (m_lRealHandle < 0)
                    iLastErr = NET_DVR_GetLastError();

                result.Message= !(m_lRealHandle < 0)?"Streaming started successfully": "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr;
                result.StatusCode = !(m_lRealHandle < 0) ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
                m_lPlayHandle = !(m_lRealHandle < 0) ? 0 : -1;
                result.Status = !(m_lRealHandle < 0);
                result.Result = lpPreviewInfo.byStreamID;
                return await Task.FromResult(result);
            }

            result.Message = "Currently streaming";
            result.StatusCode = HttpStatusCode.BadRequest;
            result.Status = false;
            _cookieStore.StoreStringData(Constants.m_lRealHandle, "-1");
            return await Task.FromResult(result);
        }

        public async Task<ApiResult> Stop()
        {
            ApiResult result = new();
            if (Convert.ToInt32(_cookieStore.GetStringData(Constants.m_lRealHandle)) >= 0)
            {
                bool isStopped = NET_DVR_StopRealPlay(m_lRealHandle);
                //Í£Ö¹Ô¤ÀÀ Stop live view 
                if (!isStopped)
                    iLastErr = NET_DVR_GetLastError();
                result.Message = isStopped ? "Stream stopped successfully" : "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                result.Status = isStopped;
                result.StatusCode = isStopped ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
                m_lRealHandle = isStopped ? -1 : 0;
                _cookieStore.StoreStringData(Constants.m_lRealHandle, "-1");
            }
            else
            {
                result.Message = "No Stream currently on";
            }
            return await Task.FromResult(result);
        }



        
    }
}
