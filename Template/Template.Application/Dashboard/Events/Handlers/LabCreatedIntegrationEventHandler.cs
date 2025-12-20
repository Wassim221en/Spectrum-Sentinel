using Microsoft.Extensions.Logging;
using Template.API.Dashboard.Events;
using Template.Dashboard.Events;
using Template.Domain.Events.Lab;

namespace Template.Dashboard.Events.Handlers;

/// <summary>
/// Handler for LabCreatedIntegrationEvent
/// This is an example handler that processes lab creation events
/// </summary>
public class LabCreatedIntegrationEventHandler : IIntegrationEventHandler<LabCreatedIntegrationEvent>
{
    private readonly ILogger<LabCreatedIntegrationEventHandler> _logger;

    public LabCreatedIntegrationEventHandler(ILogger<LabCreatedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(LabCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling LabCreatedIntegrationEvent for lab {LabId} - {Name}",
            @event.LabId,
            @event.Name);

        // Here you can implement additional logic such as:
        // - Notify other services about the new lab
        // - Initialize lab-specific resources
        // - Send notifications to administrators
        // - Update analytics or reporting systems

        _logger.LogInformation(
            "Successfully processed lab creation event for {Name}",
            @event.Name);

        return Task.CompletedTask;
    }
}

