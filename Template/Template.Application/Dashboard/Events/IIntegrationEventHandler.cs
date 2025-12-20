using Template.Domain.Events;

namespace Template.API.Dashboard.Events;

/// <summary>
/// Interface for handling integration events
/// </summary>
public interface IIntegrationEventHandler<in TEvent> where TEvent : IntegrationEvent
{
    /// <summary>
    /// Handle the integration event
    /// </summary>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

