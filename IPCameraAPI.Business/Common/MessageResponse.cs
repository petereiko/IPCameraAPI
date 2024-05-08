using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Common
{
    public class MessageResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
    }

    public class FileMessageResponseo:MessageResponse
    {
        public byte[] Video { get; set; }
    }

    public class MessageResponse<T> where T : class
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public T Result { get; set; }
    }

}
