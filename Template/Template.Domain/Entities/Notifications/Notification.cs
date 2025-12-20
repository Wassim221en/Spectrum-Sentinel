using Template.Domain.Entities.Base;

namespace Template.Domain.Entities.Notifications;

public class Notification:Entity
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; }
    public string? DataJson { get; private set; }
    private List<EmployeeNotification> _userNotifications = new();
    public IReadOnlyCollection<EmployeeNotification> UserNotifications => _userNotifications.AsReadOnly();

    // Parameterless constructor for EF Core
    private Notification()
    {
    }

    public Notification(string title,string body, string? dataJson)
    {
        Title = title;
        Body = body;
        DataJson = dataJson;
    }

    public void AddUser(Guid userId)
        => _userNotifications.Add(new EmployeeNotification(userId));

    public void AddUsers(List<Guid> userIds)
        => _userNotifications.AddRange(userIds.Select(id => new EmployeeNotification(id)));
}