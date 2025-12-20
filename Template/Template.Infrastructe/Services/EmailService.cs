using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Template.Application.Interfaces;

namespace Template.Infrastructe.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["Email:From"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var builder = new BodyBuilder();
        if (isHtml)
        {
            builder.HtmlBody = body;
        }
        else
        {
            builder.TextBody = body;
        }
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(
                _configuration["Email:SmtpServer"],
                int.Parse(_configuration["Email:Port"] ?? "587"),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _configuration["Email:Username"],
                _configuration["Email:Password"]
            );

            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken, string employeeName)
    {
        var subject = "Password Reset Request";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f9f9f9;
            border-radius: 10px;
        }}
        .header {{
            background-color: #4CAF50;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 10px 10px 0 0;
        }}
        .content {{
            background-color: white;
            padding: 30px;
            border-radius: 0 0 10px 10px;
        }}
        .token-box {{
            background-color: #f0f0f0;
            padding: 15px;
            margin: 20px 0;
            border-left: 4px solid #4CAF50;
            font-family: monospace;
            word-break: break-all;
        }}
        .footer {{
            text-align: center;
            margin-top: 20px;
            color: #666;
            font-size: 12px;
        }}
        .warning {{
            color: #ff6b6b;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Password Reset Request</h1>
        </div>
        <div class=""content"">
            <p>Hello <strong>{employeeName}</strong>,</p>
            
            <p>We received a request to reset your password. Use the token below to reset your password:</p>
            
            <div class=""token-box"">
                <strong>Reset Token:</strong><br>
                {resetToken}
            </div>
            
            <p><span class=""warning"">⚠️ Important:</span></p>
            <ul>
                <li>This token will expire in <strong>1 hour</strong></li>
                <li>If you didn't request this, please ignore this email</li>
                <li>Never share this token with anyone</li>
            </ul>
            
            <p>To reset your password, use this token in the password reset form.</p>
            
            <p>Best regards,<br>
            <strong>Template API Team</strong></p>
        </div>
        <div class=""footer"">
            <p>This is an automated email. Please do not reply to this message.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendWelcomeEmailAsync(string to, string employeeName)
    {
        var subject = "Welcome to Template API";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f9f9f9;
            border-radius: 10px;
        }}
        .header {{
            background-color: #2196F3;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 10px 10px 0 0;
        }}
        .content {{
            background-color: white;
            padding: 30px;
            border-radius: 0 0 10px 10px;
        }}
        .footer {{
            text-align: center;
            margin-top: 20px;
            color: #666;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Welcome! 🎉</h1>
        </div>
        <div class=""content"">
            <p>Hello <strong>{employeeName}</strong>,</p>
            
            <p>Welcome to Template API! Your account has been successfully created.</p>
            
            <p>You can now log in and start using our services.</p>
            
            <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
            
            <p>Best regards,<br>
            <strong>Template API Team</strong></p>
        </div>
        <div class=""footer"">
            <p>This is an automated email. Please do not reply to this message.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(to, subject, body, true);
    }
}

