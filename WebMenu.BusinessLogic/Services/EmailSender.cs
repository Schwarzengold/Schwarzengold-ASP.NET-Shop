using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using WebMenu.BusinessLogic.Interfaces;
using System.Threading.Tasks;

namespace WebMenu.BusinessLogic.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "itstepbibaandbobainc@gmail.com";
        private readonly string _smtpPass = "kvqd qtvy gtzz puvc";

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
