using Template.Domain.Entities.Base;
using Template.Domain.Entities.Security;
using Template.Domain.Primitives.Entity.Identity;

namespace Template.Domain.Entities.Notifications;

public class EmployeeNotification:Entity
{
    public Guid EmployeeId { get;private set; }
    public Employee Employee { get;private set; }
    public Guid NotificationId { get; private set; }
    public Notification Notification { get; private set; }
    public bool IsRead { get;private set; }
    public DateTime? ReadAt { get; private set; }

    // Parameterless constructor for EF Core
    private EmployeeNotification()
    {
    }

    public EmployeeNotification(Guid employeeId)
    {
        EmployeeId = employeeId;
        IsRead = false;
    }
    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}