namespace ExaltedCharm.Api.Services
{
    public interface IMailService
    {
        void Send(string subject, string message);
    }
}