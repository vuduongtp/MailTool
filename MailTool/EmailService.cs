using MailTool;
using System;
using System.Net;
using System.Net.Mail;

namespace WebApi.Helpers.Services
{
    public class EmailService
    {
        public static bool SendMail(string[] to, string[] cc, string subject, string body, string contentType, string attachment, string attachmentFileName, string sPasswordAttach)
        {
            SmtpClient smtpClient = new SmtpClient
            {
                Host = Program.Host,
                Port = Program.Port,
                Credentials = new NetworkCredential(Program.Username, Program.Password),
                EnableSsl = true
            };
            MailMessage message = new MailMessage
            {
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.Delay
            };
            message.Headers.Add("Disposition-Notification-To", Program.Username);
            message.Headers.Add("Content-Type", contentType+ ";charset=UTF-8");
            message.From = new MailAddress(Program.Username);
            foreach(string toAddress in to)
            {
                message.To.Add(toAddress);
            }
            if (cc.Length > 0)
            {
                foreach (string ccAddress in cc)
                {
                    message.CC.Add(ccAddress);
                }
            }
            message.Subject = subject;
            if(contentType == "text/html")
            {
                message.IsBodyHtml = true;
            }    
            // Message body content
            message.Body = body;
            try
            {
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}
