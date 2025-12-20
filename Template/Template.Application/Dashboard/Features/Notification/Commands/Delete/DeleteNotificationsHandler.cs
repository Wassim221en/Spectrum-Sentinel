using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Notification.Commands.Delete;

public class DeleteNotificationsHandler : IRequestHandler<DeleteNotificationsCommand.Request, OperationResponse>
{
    private readonly IRepository _repository;

    public DeleteNotificationsHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse> Handle(DeleteNotificationsCommand.Request request, CancellationToken cancellationToken)
    {
        if (!request.NotificationIds.Any())
            return new HttpMessage("No notification IDs provided", HttpStatusCode.BadRequest);

        var notifications = await _repository.TrackingQuery<Domain.Entities.Notifications.Notification>()
            .Where(n => request.NotificationIds.Contains(n.Id))
            .ToListAsync(cancellationToken);

        if (!notifications.Any())
            return new HttpMessage("No notifications found", HttpStatusCode.NotFound);

        _repository.SoftDeleteRange(notifications);
        await _repository.SaveChangesAsync(cancellationToken);

        return OperationResponse.Ok();
    }
}

