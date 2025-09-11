
using System.Net;
using System.Net.Mail;

namespace City_Bus_Management_System.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromEmail = "youssefmohmed430@gmail.com";
            var fromPassword = "YusefmohmedGaber2005";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            return client.SendMailAsync(
                new MailMessage(from : fromEmail,to : toEmail, subject, body)
            );
        }
    }
}
