using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Notification.Queries.GetById;

public class GetNotificationByIdHandler : IRequestHandler<GetNotificationByIdQuery.Request, OperationResponse<GetNotificationByIdQuery.Response>>
{
    private readonly IRepository _repository;

    public GetNotificationByIdHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetNotificationByIdQuery.Response>> Handle(GetNotificationByIdQuery.Request request, CancellationToken cancellationToken)
    {
        var notification = await _repository.Query<Domain.Entities.Notifications.Notification>()
            .Include(n => n.UserNotifications)
                .ThenInclude(un => un.Employee)
            .Where(n => n.Id == request.Id)
            .Select(GetNotificationByIdQuery.Response.Selector())
            .FirstOrDefaultAsync(cancellationToken);

        if (notification is null)
            return new HttpMessage("Notification not found", HttpStatusCode.NotFound);

        return notification;
    }
}

