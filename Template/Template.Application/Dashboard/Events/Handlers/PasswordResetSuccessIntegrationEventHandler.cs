using Microsoft.Extensions.Logging;
using Template.API.Dashboard.Events;
using Template.Application.Interfaces;
using Template.Dashboard.Events;
using Template.Domain.Events.Employee;

namespace Template.Application.Events.Handlers;

public class PasswordResetSuccessIntegrationEventHandler : IIntegrationEventHandler<PasswordResetSuccessIntegrationEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordResetSuccessIntegrationEventHandler> _logger;

    public PasswordResetSuccessIntegrationEventHandler(
        IEmailService emailService,
        ILogger<PasswordResetSuccessIntegrationEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(PasswordResetSuccessIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling PasswordResetSuccessIntegrationEvent for employee: {Email}",
            @event.Email);

        try
        {
            // Send confirmation email
            await _emailService.SendEmailAsync(
                @event.Email,
                "Password Reset Successful",
                $"Hello {@event.FullName},\n\n" +
                $"Your password has been successfully reset.\n\n" +
                $"If you did not perform this action, please contact support immediately.\n\n" +
                $"Best regards,\n" +
                $"Template Team");

            _logger.LogInformation(
                "Password reset confirmation email sent successfully to: {Email}",
                @event.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send password reset confirmation email to: {Email}",
                @event.Email);
        }
    }
}

