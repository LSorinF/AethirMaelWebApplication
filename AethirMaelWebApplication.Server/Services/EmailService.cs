using System.Net;
using System.Net.Mail;

namespace AethirMaelWebApplication.Server.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var settings = _configuration.GetSection("EmailSettings");

            var smtpClient = new SmtpClient(settings["SmtpServer"])
            {
                Port = int.Parse(settings["SmtpPort"]),
                Credentials = new NetworkCredential(settings["SenderEmail"], settings["Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(settings["SenderEmail"], settings["SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EmailService] Email trimis catre {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Eroare: {ex.Message}");
            }
        }
    }
}
