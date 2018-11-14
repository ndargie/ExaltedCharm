using System.Diagnostics;
using ExaltedCharm.Api.Models;
using Microsoft.Extensions.Options;

namespace ExaltedCharm.Api.Services
{
    public class LocalMailService : IMailService
    {
        public LocalMailService(IOptions<MailSettings> mailSettings)
        {
            _mailFrom = mailSettings.Value.MailFromAddress;
            _mailTo = mailSettings.Value.MailToAddress;
        }

        private readonly string _mailTo;
        private readonly string _mailFrom;

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with LocalMailService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}