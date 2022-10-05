using BugTracker.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BugTracker.Services
{
    public class EmailService : IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public Task SendEmailAsync(BTUser btUser, EmailData emailData)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress");

            MimeMessage newEmail = new MimeMessage();

            //add all email addresses to the "TO" for the email
            newEmail.Sender = MailboxAddress.Parse(emailSender);
            newEmail.To.Add(MailboxAddress.Parse(email));

            //add the subject for the email
            newEmail.Subject = subject;

            //add the body for the email
            BodyBuilder emailBody = new BodyBuilder();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            //send the email
            using SmtpClient smtpClient = new SmtpClient();
            try
            {
                var host = _mailSettings.EmailHost ?? Environment.GetEnvironmentVariable("Host");
                var port = _mailSettings.EmailPort != 0 ? _mailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("Port")!);
                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, _mailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("Password"));

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);
            }
            catch
            {
                throw;
            }
        }
    }
}