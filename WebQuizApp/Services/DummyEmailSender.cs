// Services/DummyEmailSender.cs
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebQuizApp.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Do nothing - we're not actually sending emails
            return Task.CompletedTask;
        }
    }
}
