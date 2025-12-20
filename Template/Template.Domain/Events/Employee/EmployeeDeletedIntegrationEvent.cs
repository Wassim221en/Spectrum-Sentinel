namespace Template.Domain.Events.Employee;

/// <summary>
/// Integration event published when an employee is deleted
/// </summary>
public class EmployeeDeletedIntegrationEvent : IntegrationEvent
{
    public Guid EmployeeId { get; private set; }
    public string Email { get; private set; }

    public EmployeeDeletedIntegrationEvent(Guid employeeId, string email)
    {
        EmployeeId = employeeId;
        Email = email;
    }
}

