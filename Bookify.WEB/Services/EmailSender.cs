using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Bookify.WEB.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmailSender(IOptions<MailSettings> mailSettings, IWebHostEnvironment webHostEnvironment)
        {
            _mailSettings = mailSettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage message = new()
            {
                From = new MailAddress(_mailSettings.Email!,_mailSettings.DisplayName),
                Body = htmlMessage,
                Subject = subject,
                IsBodyHtml = true
            };
            message.To.Add(_webHostEnvironment.IsDevelopment()?"wetogo8429@nmaller.com":email);
            SmtpClient smtpClient = new(_mailSettings.Host)
            {
                Port = int.Parse(_mailSettings.Port!),
                Credentials = new NetworkCredential(_mailSettings.Email,_mailSettings.Password),
                EnableSsl = true
            };
            await smtpClient.SendMailAsync(message);
            smtpClient.Dispose();
        }
    }
}
