using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Notification.Queries.GetAll;

public class GetAllNotificationsQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        public string? Search { get; set; }
        public Guid? UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class Response
    {
        public int Count { get; set; }
        public List<NotificationRes> Notifications { get; set; } = new();

        public class NotificationRes
        {
            public Guid NotificationId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
            public string? DataJson { get; set; }
            public DateTimeOffset DateCreated { get; set; }
            public int UserCount { get; set; }

            public static Expression<Func<Domain.Entities.Notifications.Notification, NotificationRes>> Selector() => n => new()
            {
                NotificationId = n.Id,
                Title = n.Title,
                Body = n.Body,
                DataJson = n.DataJson,
                DateCreated = n.DateCreated,
                UserCount = n.UserNotifications.Count
            };
        }
    }
}

