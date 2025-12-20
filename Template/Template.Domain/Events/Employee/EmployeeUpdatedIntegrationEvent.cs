namespace Template.Domain.Events.Employee;

/// <summary>
/// Integration event published when an employee is updated
/// </summary>
public class EmployeeUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid EmployeeId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }

    public EmployeeUpdatedIntegrationEvent(
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

