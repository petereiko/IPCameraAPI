using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Common
{
    public class ApiResult<T>
    {
        public T Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }

    public class ApiResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
