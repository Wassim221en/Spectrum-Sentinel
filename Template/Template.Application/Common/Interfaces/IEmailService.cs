namespace Template.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendPasswordResetEmailAsync(string to, string resetToken, string employeeName);
    Task SendWelcomeEmailAsync(string to, string employeeName);
}

