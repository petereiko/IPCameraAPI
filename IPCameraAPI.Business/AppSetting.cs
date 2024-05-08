using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business
{
    public class AppSetting
    {
        public string VideoPath { get; set; }
        public string ImagePath { get; set; }
        public EmailSetting EmailSetting { get; set; }
    }

    public class EmailSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
