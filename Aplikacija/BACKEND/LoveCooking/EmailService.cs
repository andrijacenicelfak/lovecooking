using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace LoveCooking
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html);
    }

    public class EmailService : IEmailService
    {
        private string? mail, password, host, port;
        public EmailService(string mail, string password, string host, string port)
        {
            this.mail = mail;
            this.password = password;
            this.host = host;
            this.port = port;
        }

        public void Send(string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(mail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            smtp.Connect(host, Int32.Parse(port!), SecureSocketOptions.StartTls);
            smtp.Authenticate(mail, password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}