namespace Template.Domain.Events.Employee;

/// <summary>
/// Integration event published when a new employee is created
/// </summary>
public class EmployeeCreatedIntegrationEvent : IntegrationEvent
{
    public Guid EmployeeId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }

    public EmployeeCreatedIntegrationEvent(
        Guid employeeId,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber)
    {
        EmployeeId = employeeId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}

