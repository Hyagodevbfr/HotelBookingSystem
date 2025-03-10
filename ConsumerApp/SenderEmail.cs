using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerApp;
internal class SenderEmail
{
    public static void SendEmail(string toEmail,string subject,string htmlContent)
    {
        var configSmtp = new ConfigSmtp( );
        var smtpClient = new SmtpClient(configSmtp.smtpClient)
        {
            Port = 587,
            Credentials = new NetworkCredential(configSmtp.Email, configSmtp.PasswordApp),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(configSmtp.Email),
            Subject = subject,
            Body = htmlContent,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);
        smtpClient.Send(mailMessage);
    }
}
