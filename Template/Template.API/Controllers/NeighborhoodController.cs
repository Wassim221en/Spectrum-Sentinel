using MediatR;
using Microsoft.AspNetCore.Mvc;
using Template.API.Attributes;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Neighborhood.Commands.Add;
using Template.Dashboard.Neighborhood.Commands.Delete;
using Template.Dashboard.Neighborhood.Commands.Modify;
using Template.Dashboard.Neighborhood.Queries.GetAll;
using Template.Dashboard.Neighborhood.Queries.GetById;

namespace Template.API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[ApiGroup<SampleApiGroup>(SampleApiGroup.Dashboard)]
public class NeighborhoodController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NeighborhoodController> _logger;

    public NeighborhoodController(IMediator mediator, ILogger<NeighborhoodController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetAllNeighborhoodsQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllNeighborhoodsQuery.Request request,
        [FromServices] IRequestHandler<GetAllNeighborhoodsQuery.Request, OperationResponse<GetAllNeighborhoodsQuery.Response>> handler)
        => Ok(await handler.Handle(request, CancellationToken.None));

    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetNeighborhoodByIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromQuery] GetNeighborhoodByIdQuery.Request request,
        [FromServices] IRequestHandler<GetNeighborhoodByIdQuery.Request, OperationResponse<GetNeighborhoodByIdQuery.Response>> handler)
        => Ok(await handler.Handle(request, CancellationToken.None));

    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse<GetAllNeighborhoodsQuery.Response.NeighborhoodRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] AddNeighborhoodCommand.Request request,
        [FromServices] IRequestHandler<AddNeighborhoodCommand.Request, OperationResponse<GetAllNeighborhoodsQuery.Response.NeighborhoodRes>> handler)
        => Ok(await handler.Handle(request, CancellationToken.None));

    [HttpPut]
    [ProducesResponseType(typeof(OperationResponse<GetNeighborhoodByIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Modify([FromBody] ModifyNeighborhoodCommand.Request request,
        [FromServices] IRequestHandler<ModifyNeighborhoodCommand.Request, OperationResponse<GetNeighborhoodByIdQuery.Response>> handler)
        => Ok(await handler.Handle(request, CancellationToken.None));

    [HttpDelete]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromBody] DeleteNeighborhoodsCommand.Request request,
        [FromServices] IRequestHandler<DeleteNeighborhoodsCommand.Request, OperationResponse> handler)
        => Ok(await handler.Handle(request, CancellationToken.None));
}

