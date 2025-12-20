using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Notification.Queries.GetById;

public class GetNotificationByIdQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        public Guid Id { get; set; }
    }

    public class Response
    {
        public Guid NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? DataJson { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public List<UserNotificationInfo> Users { get; set; } = new();

        public class UserNotificationInfo
        {
            public Guid UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public bool IsRead { get; set; }
            public DateTime? ReadAt { get; set; }
        }

        public static Expression<Func<Domain.Entities.Notifications.Notification, Response>> Selector() => n => new()
        {
            NotificationId = n.Id,
            Title = n.Title,
            Body = n.Body,
            DataJson = n.DataJson,
            DateCreated = n.DateCreated,
            Users = n.UserNotifications.Select(un => new UserNotificationInfo
            {
                UserId = un.EmployeeId,
                UserName = un.Employee.UserName ?? string.Empty,
                Email = un.Employee.Email ?? string.Empty,
                IsRead = un.IsRead,
                ReadAt = un.ReadAt
            }).ToList()
        };
    }
}

