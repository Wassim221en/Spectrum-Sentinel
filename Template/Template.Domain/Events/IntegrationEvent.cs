namespace Template.Domain.Events;

/// <summary>
/// Base class for integration events that are published to external systems via message broker
/// </summary>
public abstract class IntegrationEvent
{
    /// <summary>
    /// Unique identifier for this event
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Timestamp when the event was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// Event type name for routing and deserialization
    /// </summary>
    public string EventType { get; private set; }

    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        EventType = GetType().Name;
    }

    protected IntegrationEvent(Guid id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
        EventType = GetType().Name;
    }
}

