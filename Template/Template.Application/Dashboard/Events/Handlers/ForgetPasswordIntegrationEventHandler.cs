using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Template.API.Dashboard.Events;
using Template.Application.Interfaces;
using Template.Domain.Events.Employee;

namespace Template.Dashboard.Events.Handlers;

public class ForgetPasswordIntegrationEventHandler:IIntegrationEventHandler<ForgetPasswordIntegrationEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgetPasswordIntegrationEventHandler> _logger;

    public ForgetPasswordIntegrationEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task HandleAsync(ForgetPasswordIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling ${event}",this.GetType().Name);
        await _emailService.SendPasswordResetEmailAsync(@event.Email, @event.ResetPswToken, @event.FullName);
    }
}