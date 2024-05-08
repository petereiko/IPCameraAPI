using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.DTOs
{
    public class EmailRequest
    {
        public string Subject { get; set; }
        public string Msg { get; set; }
        public string Recipient { get; set; }
        public List<string> AttachmentFiles { get; set; }=new List<string>();
        public List<string> Copies { get; set; }=new List<string>();
        public List<string> BCopies { get; set; } = new List<string>();
    }
}
