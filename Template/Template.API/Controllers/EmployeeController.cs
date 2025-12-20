using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.API;
using Template.API.Attributes;
using Template.API.Authorization;
using Template.API.Employee.Commands.Auth.Commands.ForgetPassword;
using Template.API.Employee.Commands.Auth.Commands.Login;
using Template.API.Employee.Commands.Auth.Commands.RefreshToken;
using Template.Application.Features.Auth.Commands.ResetPassword;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Employee.Commands.Add;
using Template.Dashboard.Employee.Commands.Delete;
using Template.Dashboard.Employee.Commands.Modify;
using Template.Dashboard.Employee.Queries.GetAll;
using Template.Dashboard.Employee.Queries.GetById;

[ApiController]
[Route("api/[controller]/[action]")]
[ApiGroup<SampleApiGroup>(SampleApiGroup.Dashboard)]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(IMediator mediator, ILogger<EmployeeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand.Request request,
        [FromServices] IRequestHandler<LoginCommand.Request, OperationResponse<LoginCommand.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgetPassword(
        [FromBody] ForgetPasswordCommand.Request request,
        [FromServices] IRequestHandler<ForgetPasswordCommand.Request, OperationResponse> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand.Request request,
        [FromServices] IRequestHandler<ResetPasswordCommand.Request, OperationResponse> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenCommand.Request request,
        [FromServices]
        IRequestHandler<RefreshTokenCommand.Request, OperationResponse<RefreshTokenCommand.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpGet]
    [HasPermissions("Users.View", "Users.Edit")]
    [ProducesResponseType(typeof(OperationResponse<GetAllEmployeesQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllEmployeesQuery.Request request,
        [FromServices] IRequestHandler<GetAllEmployeesQuery.Request, OperationResponse<GetAllEmployeesQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetEmployeeByIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromQuery] GetEmployeeByIdQuery.Request request,
        [FromServices] IRequestHandler<GetEmployeeByIdQuery.Request, OperationResponse<GetEmployeeByIdQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse<GetAllEmployeesQuery.Response.EmployeeRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(
        [FromForm] AddEmployeeCommand.Request request,
        [FromServices]
        IRequestHandler<AddEmployeeCommand.Request, OperationResponse<GetAllEmployeesQuery.Response.EmployeeRes>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPut]
    [ProducesResponseType(typeof(OperationResponse<GetEmployeeByIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Modify(
        [FromBody] ModifyEmployeeCommand.Request request,
        [FromServices]
        IRequestHandler<ModifyEmployeeCommand.Request, OperationResponse<GetEmployeeByIdQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpDelete]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteEmployeesCommand.Request request,
        [FromServices]
        IRequestHandler<DeleteEmployeesCommand.Request, OperationResponse> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();
}
