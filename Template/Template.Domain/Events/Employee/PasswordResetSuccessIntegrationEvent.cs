namespace Template.Domain.Events.Employee;

public class PasswordResetSuccessIntegrationEvent : IntegrationEvent
{
    public string FullName { get; private set; }
    public string Email { get; private set; }

    public PasswordResetSuccessIntegrationEvent(string fullName, string email)
    
    {
        FullName = fullName;
        Email = email;
    }
}

