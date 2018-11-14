using System.Diagnostics;
using ExaltedCharm.Api.Models;
using Microsoft.Extensions.Options;

namespace ExaltedCharm.Api.Services
{
    public class CloudMailService : IMailService
    {
        private readonly string _mailTo;
        private readonly string _mailFrom;

        public CloudMailService(IOptions<MailSettings> mailSettings)
        {
            _mailFrom = mailSettings.Value.MailFromAddress;
            _mailTo = mailSettings.Value.MailToAddress;
        }

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with CloudMailService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}