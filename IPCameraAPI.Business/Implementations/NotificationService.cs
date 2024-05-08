using IPCameraAPI.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Mime;
using Microsoft.Extensions.Options;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Common;
using IPCameraAPI.Business.Validations;
using Microsoft.AspNetCore.Hosting;

namespace IPCameraAPI.Business.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly AppSetting _appSetting;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public NotificationService(IOptions<AppSetting> options, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment)
        {
            _appSetting = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }
        public Task SendEmail(string message, string email, string title)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailWithAttachment(string message, List<string> attachments, string email, string title)
        {
            EmailRequest emailRequest = new() { AttachmentFiles = attachments, Msg = message, Recipient = email, Subject = title };
            await SendHTMLMail(emailRequest);
        }

        private async Task<bool> SendHTMLMail(EmailRequest request)
        {
            bool result = false;
            MailMessage message = new MailMessage();
            string err = "";
            bool cont = true;
            try
            {
                var validator = new EmailRequestValidation();
                var validationResult = validator.Validate(request);

                if (!validationResult.IsValid)
                {
                    var objResult = new ApiResult<MessageResponse>()
                    {
                        Message = validationResult.Errors.FirstOrDefault()?.ErrorMessage,
                        Result = new() { Message = "Validation Error", Status = false }
                    };
                    return false;
                }

                SmtpClient smtpclient;
                smtpclient = new SmtpClient();
                smtpclient.Host = _appSetting.EmailSetting.Host;//Configurations.getConfigurationValue("Mail_Server");
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Images", "swift_logo.png"); //context.Server.MapPath(@"images/" + imageurl); // my logo is placed in images folder

                message.From = new MailAddress(_appSetting.EmailSetting.Email);//  Configurations.getConfigurationValue("Mail_From"));
                message.To.Add(request.Recipient);
                if (request.Copies.Count > 0)
                {
                    foreach (string item in request.Copies)
                    {
                        message.CC.Add(item);
                    }
                }
                if (request.BCopies.Count > 0)
                {
                    foreach (string item in request.BCopies)
                    {
                        message.Bcc.Add(item);
                    }
                }

                if (cont)
                {
                    message.Subject = request.Subject;
                    LinkedResource logo = new LinkedResource(path);
                    logo.ContentId = "swiftlogo";

                    AlternateView av1 = AlternateView.CreateAlternateViewFromString("<html><body>" + request.Msg.Replace("imgswiftlogo", "<img src=cid:swiftlogo/>") + "<br /><br />Disclaimer: This message and any files transmitted with it are confidential and privileged. If you have received it in error, please notify the sender by return e-mail and delete this message from your system. If you are not the intended recipient you are hereby notified that any dissemination, copy or disclosure of this e-mail is strictly prohibited.</body></html>", null, MediaTypeNames.Text.Html);
                    av1.LinkedResources.Add(logo);


                    message.AlternateViews.Add(av1);
                    //message.Body = msg;
                    if (request.AttachmentFiles.Count>0)
                    {
                        foreach (string item in request.AttachmentFiles)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                Attachment attach = new Attachment(item);
                                message.Attachments.Add(attach);
                            }
                        }
                    }
                    message.IsBodyHtml = true;
                    smtpclient.SendAsync(message, null);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }
            finally
            {
                message.Dispose();
            }

            return await Task.FromResult(result);
        }

        public async Task SendSms(string message, string phone)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://messaging.updigital-ng.com/smsapi/index?key=464BE56EAE17B5&campaign=0&routeid=1&type=text&contacts={phone}&senderid=Atlas&msg={message}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());

        }
    }
}
