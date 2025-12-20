using Template.Dashboard.Notification.Queries.GetAll;
using Template.Domain.Specifications;

namespace Template.Dashboard.Dashboard.Features.Notification.Queries.GetAll;

public class GetAllNotificationSpecification:Specification<Domain.Entities.Notifications.Notification>
{
    public GetAllNotificationSpecification(GetAllNotificationsQuery.Request request)
    {
        ApplyFilters(n=>n.UserNotifications.Any(un => un.EmployeeId == request.UserId)
        &&(request.Search==""||request.Search==null||n.Title.Contains(request.Search)));
    }
}