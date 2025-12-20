using Microsoft.Extensions.Logging;
using Template.API.Dashboard.Events;
using Template.Dashboard.Events;
using Template.Domain.Events.Employee;

namespace Template.Dashboard.Events.Handlers;

/// <summary>
/// Handler for EmployeeDeletedIntegrationEvent
/// This is an example handler that logs when an employee is deleted
/// </summary>
public class EmployeeDeletedIntegrationEventHandler : IIntegrationEventHandler<EmployeeDeletedIntegrationEvent>
{
    private readonly ILogger<EmployeeDeletedIntegrationEventHandler> _logger;

    public EmployeeDeletedIntegrationEventHandler(ILogger<EmployeeDeletedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(EmployeeDeletedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling EmployeeDeletedIntegrationEvent for employee {EmployeeId} - {Email}",
            @event.EmployeeId,
            @event.Email);

        // Here you can implement additional logic such as:
        // - Notify other services about the deletion
        // - Clean up related data in other systems
        // - Archive employee data
        // - Send notification to administrators

        _logger.LogInformation(
            "Successfully processed employee deletion event for {Email}",
            @event.Email);

        return Task.CompletedTask;
    }
}

