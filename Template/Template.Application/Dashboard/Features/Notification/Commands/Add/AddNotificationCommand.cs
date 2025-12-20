using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Notification.Queries.GetAll;

namespace Template.Dashboard.Notification.Commands.Add;

public class AddNotificationCommand
{
    public class Request : IRequest<OperationResponse<GetAllNotificationsQuery.Response.NotificationRes>>
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? DataJson { get; set; }
        public List<Guid>? EmployeeIds { get; set; } = new();
        public bool SendToAllEmployees { get; set; } = false;

    }
}

