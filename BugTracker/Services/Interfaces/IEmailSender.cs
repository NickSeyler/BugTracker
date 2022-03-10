namespace BugTracker.Services.Interfaces
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string emailTo, string subject, string htmlMessage);
    }
}
