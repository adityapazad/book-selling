using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Project._02.Utility
{
    public class EmailSender : IEmailSender
    {
        private EmailSettings _emailSettings {  get; }
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task Execute(string email, string subject, string message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.FromEmail : email;

                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "My Email Name")
                };
                mailMessage.To.Add(toEmail);
                mailMessage.CC.Add(_emailSettings.CcEmail);
                mailMessage.Subject = "Shopping App: " + subject;
                mailMessage.Body = message;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.High;
                using (SmtpClient smtpClient = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtpClient.Send(mailMessage);
                };
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Execute(email, subject, htmlMessage).Wait();
            return Task.FromResult(0);
        }
        
    }
}
