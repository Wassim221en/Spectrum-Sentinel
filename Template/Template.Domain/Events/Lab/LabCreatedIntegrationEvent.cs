namespace Template.Domain.Events.Lab;

/// <summary>
/// Integration event published when a new lab is created
/// </summary>
public class LabCreatedIntegrationEvent : IntegrationEvent
{
    public Guid LabId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    public LabCreatedIntegrationEvent(Guid labId, string name, string? description)
    {
        LabId = labId;
        Name = name;
        Description = description;
    }
}

