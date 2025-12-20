using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Notification.Queries.GetAll;

public class GetAllNotificationsHandler : IRequestHandler<GetAllNotificationsQuery.Request, OperationResponse<GetAllNotificationsQuery.Response>>
{
    private readonly IRepository _repository;

    public GetAllNotificationsHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetAllNotificationsQuery.Response>> Handle(GetAllNotificationsQuery.Request request, CancellationToken cancellationToken)
    {
        var query = _repository.Query<Domain.Entities.Notifications.Notification>();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(n => n.Title.Contains(request.Search) || n.Body.Contains(request.Search));
        }
        
        if (request.UserId.HasValue)
        {
            query = query.Where(n => n.UserNotifications.Any(un => un.EmployeeId == request.UserId.Value));
        }

        var count = await query.CountAsync(cancellationToken);

        var notifications = await query
            .OrderByDescending(n => n.DateCreated)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(GetAllNotificationsQuery.Response.NotificationRes.Selector())
            .ToListAsync(cancellationToken);

        return new GetAllNotificationsQuery.Response
        {
            Count = count,
            Notifications = notifications
        };
    }
}

