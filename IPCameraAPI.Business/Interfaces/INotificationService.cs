using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Interfaces
{
    public interface INotificationService
    {
        Task SendSms(string message, string phone);
        Task SendEmail(string message, string email, string title);
        Task SendEmailWithAttachment(string message, List<string> attachment, string email, string title);
    }
}
