using MediatR;
using Microsoft.AspNetCore.Mvc;
using Template.API;
using Template.API.Attributes;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Dashboard.Features.Role.Queries.GetAllNames;
using Template.Dashboard.Dashboard.Features.Role.Queries.GetAllPermissions;
using Template.Dashboard.Role.Commands.Add;
using Template.Dashboard.Role.Commands.Delete;
using Template.Dashboard.Role.Commands.Modify;
using Template.Dashboard.Role.Queries.GetAll;
using Template.Dashboard.Role.Queries.GetById;

[ApiController]
[Route("api/[controller]/[action]")]
[ApiGroup<SampleApiGroup>(SampleApiGroup.Dashboard)]
public class RoleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RoleController> _logger;

    public RoleController(IMediator mediator, ILogger<RoleController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse<GetAllRolesQuery.Response.RoleRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(
        [FromBody] AddRoleCommand.Request request,
        [FromServices] IRequestHandler<AddRoleCommand.Request, OperationResponse<GetAllRolesQuery.Response.RoleRes>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetAllRolesQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllRolesQuery.Request request,
        [FromServices] IRequestHandler<GetAllRolesQuery.Request, OperationResponse<GetAllRolesQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetRolebyIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromQuery] GetRolebyIdQuery.Request request,
        [FromServices] IRequestHandler<GetRolebyIdQuery.Request, OperationResponse<GetRolebyIdQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPut]
    [ProducesResponseType(typeof(OperationResponse<GetRolebyIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Modify(
        [FromBody] ModifyRoleCommand.Request request,
        [FromServices] IRequestHandler<ModifyRoleCommand.Request, OperationResponse<GetRolebyIdQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpDelete]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteRolesCommand.Request request,
        [FromServices] IRequestHandler<DeleteRolesCommand.Request, OperationResponse> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();
    
    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetAllPermissionsQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllPermissions(
        [FromServices]
        IRequestHandler<GetAllPermissionsQuery.Request, OperationResponse<GetAllPermissionsQuery.Response>> handler)
        => (await handler.Handle(new GetAllPermissionsQuery.Request(), CancellationToken.None)).ToActionResult();

    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<List<GetAllRolesNameQuery.Response>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllNames([FromQuery] GetAllRolesNameQuery.Request request,
        [FromServices]
        IRequestHandler<GetAllRolesNameQuery.Request, OperationResponse<List<GetAllRolesNameQuery.Response>>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();
}
