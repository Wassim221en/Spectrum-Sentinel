using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.API.Dashboard.Events;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Notification.Commands.Add;
using Template.Dashboard.Notification.Queries.GetAll;
using Template.Domain.Events.Notification;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Dashboard.Features.Notification.Commands.Add;

public class AddNotificationHandler : IRequestHandler<AddNotificationCommand.Request,
    OperationResponse<GetAllNotificationsQuery.Response.NotificationRes>>
{
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;

    public AddNotificationHandler(IRepository repository, IEventBus eventBus)
    {
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<OperationResponse<GetAllNotificationsQuery.Response.NotificationRes>> Handle(
        AddNotificationCommand.Request request, CancellationToken cancellationToken)
    {
        var existingEmployees = new List<Domain.Entities.Security.Employee>();
        if (request.SendToAllEmployees is true)
            existingEmployees =
                await _repository.Query<Domain.Entities.Security.Employee>().ToListAsync(cancellationToken);
        if (request.SendToAllEmployees is false && request.EmployeeIds is not null && request.EmployeeIds.Any())
            existingEmployees = await _repository.Query<Domain.Entities.Security.Employee>()
                .Where(u => request.EmployeeIds.Contains(u.Id))
                .ToListAsync(cancellationToken);
        
        var notification = new Domain.Entities.Notifications.Notification(
            request.Title,
            request.Body,
            request.DataJson
        );
        var existingEmployeeIds = existingEmployees.Select(e => e.Id).ToList();
        notification.AddUsers(existingEmployeeIds);
        await _repository.AddAsync(notification);
        await _repository.SaveChangesAsync(cancellationToken);
        /*
        var deviceTokens = existingEmployees.SelectMany(e=>e.DeviceTokens).Where(e => e != null && e.Length > 10)
            .Select(e => e.DeviceToken).ToList();
        if (deviceTokens.Count > 0)
        {
            var sendNotificationIntegrationEvent = new SendNotificationIntegrationEvent(deviceTokens, request.Title,
                request.Body, null);
            await _eventBus.PublishAsync(sendNotificationIntegrationEvent, cancellationToken);
        }
        */

        return new GetAllNotificationsQuery.Response.NotificationRes
        {
            NotificationId = notification.Id,
            Title = notification.Title,
            Body = notification.Body,
            DataJson = notification.DataJson,
            DateCreated = notification.DateCreated,
            UserCount = existingEmployeeIds.Count
        };
    }
}

