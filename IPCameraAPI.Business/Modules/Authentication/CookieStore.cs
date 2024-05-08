using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Modules.Authentication
{
    public class CookieStore : ICookieStore
    {
        private readonly IHttpContextAccessor _accessor;

        public CookieStore(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        public int m_lUserID
        {
            get
            {

                if (_accessor.HttpContext.Request.Cookies.TryGetValue("m_lUserID", out string cookieValue))
                {
                    return Convert.ToInt32(cookieValue);
                }
                return -1;
            }
        }

        public string GetStringData(string key)
        {
            if (_accessor.HttpContext.Request.Cookies.TryGetValue(key, out string cookieValue))
            {  
                return cookieValue;
            }
            return "-1";
        }

        public void StoreStringData(string key, string data)
        {
            _accessor.HttpContext.Response.Cookies.Append(key, data);
        }

        public void RemoveData(string key)
        {
            _accessor.HttpContext.Response.Cookies.Delete(key);
        }

        


        public bool GetBooleanData(string key)
        {
            if (_accessor.HttpContext.Request.Cookies.TryGetValue(key, out string cookieValue))
            {
                return Convert.ToBoolean(cookieValue);
            }
            return false;
        }

        public void StoreBooleanData(string key, bool data)
        {
            _accessor.HttpContext.Response.Cookies.Append(key, data.ToString());
        }
    }
}
