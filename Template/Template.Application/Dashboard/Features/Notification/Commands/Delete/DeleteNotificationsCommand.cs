using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Notification.Commands.Delete;

public class DeleteNotificationsCommand
{
    public class Request : IRequest<OperationResponse>
    {
        public List<Guid> NotificationIds { get; set; } = new();
    }
}

