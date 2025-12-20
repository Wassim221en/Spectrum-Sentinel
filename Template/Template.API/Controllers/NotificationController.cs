using MediatR;
using Microsoft.AspNetCore.Mvc;
using Template.API;
using Template.API.Attributes;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Notification.Commands.Add;
using Template.Dashboard.Notification.Commands.Delete;
using Template.Dashboard.Notification.Queries.GetAll;
using Template.Dashboard.Notification.Queries.GetById;

[ApiController]
[Route("api/[controller]/[action]")]
[ApiGroup<SampleApiGroup>(SampleApiGroup.Dashboard)]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(IMediator mediator, ILogger<NotificationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all notifications with pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetAllNotificationsQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllNotificationsQuery.Request request,
        [FromServices]
        IRequestHandler<GetAllNotificationsQuery.Request, OperationResponse<GetAllNotificationsQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    /// <summary>
    /// Get notification by ID with user details
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetNotificationByIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromQuery] GetNotificationByIdQuery.Request request,
        [FromServices]
        IRequestHandler<GetNotificationByIdQuery.Request, OperationResponse<GetNotificationByIdQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    /// <summary>
    /// Create a new notification and send to specified users
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse<GetAllNotificationsQuery.Response.NotificationRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(
        [FromBody] AddNotificationCommand.Request request,
        [FromServices]
        IRequestHandler<AddNotificationCommand.Request, OperationResponse<GetAllNotificationsQuery.Response.NotificationRes>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    /// <summary>
    /// Delete one or more notifications
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteNotificationsCommand.Request request,
        [FromServices]
        IRequestHandler<DeleteNotificationsCommand.Request, OperationResponse> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();
    
}
