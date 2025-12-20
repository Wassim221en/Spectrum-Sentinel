using Template.Domain.Events;

namespace Template.API.Dashboard.Events;

/// <summary>
/// Interface for publishing and subscribing to integration events
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publish an integration event to the message broker
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IntegrationEvent;

    /// <summary>
    /// Subscribe to an integration event
    /// </summary>
    void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;

    /// <summary>
    /// Unsubscribe from an integration event
    /// </summary>
    void Unsubscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;

    /// <summary>
    /// Start consuming messages from the message broker
    /// </summary>
    void StartConsuming();

    /// <summary>
    /// Stop consuming messages from the message broker
    /// </summary>
    void StopConsuming();
}

