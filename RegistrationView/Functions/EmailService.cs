using System.Net.Mail;
using System.Net;

namespace RegistrationView.Functions
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService()
        {
            _smtpClient = new SmtpClient("mail.lis-demos.co.za", 587)
            {
                Credentials = new NetworkCredential("balu@lis-demos.co.za", "Test@12345#TTf"),
                EnableSsl = true
            };
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage("balu@lis-demos.co.za", to, subject, body);
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
