namespace Template.Domain.Events.Employee;

public class ForgetPasswordIntegrationEvent:IntegrationEvent
{
    public string FullName { get; set; }
    public string ResetPswToken { get; set; }
    public string Email { get; set; }

    public ForgetPasswordIntegrationEvent(string fullName, string resetPswToken, string email)
    {
        FullName = fullName;
        ResetPswToken = resetPswToken;
        Email = email;
    }
}