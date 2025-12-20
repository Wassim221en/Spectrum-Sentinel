using MediatR;
using Microsoft.AspNetCore.Mvc;
using Template.API;
using Template.API.Attributes;
using Template.Dashboard.City.Commands.Add;
using Template.Dashboard.City.Commands.Delete;
using Template.Dashboard.City.Commands.Modify;
using Template.Dashboard.City.Queries.GetAll;
using Template.Dashboard.City.Queries.GetById;
using Template.Dashboard.Core.Response;

[ApiController]
[Route("api/[controller]/[action]")]
[ApiGroup<SampleApiGroup>(SampleApiGroup.Dashboard)]
public class CityController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CityController> _logger;

    public CityController(IMediator mediator, ILogger<CityController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetAllCitiesQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllCitiesQuery.Request request,
        [FromServices] IRequestHandler<GetAllCitiesQuery.Request, OperationResponse<GetAllCitiesQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetCityByIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromQuery] GetCityByIdQuery.Request request,
        [FromServices] IRequestHandler<GetCityByIdQuery.Request, OperationResponse<GetCityByIdQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse<GetAllCitiesQuery.Response.CityRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(
        [FromBody] AddCityCommand.Request request,
        [FromServices] IRequestHandler<AddCityCommand.Request, OperationResponse<GetAllCitiesQuery.Response.CityRes>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPut]
    [ProducesResponseType(typeof(OperationResponse<GetCityByIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Modify(
        [FromBody] ModifyCityCommand.Request request,
        [FromServices] IRequestHandler<ModifyCityCommand.Request, OperationResponse<GetCityByIdQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpDelete]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteCitiesCommand.Request request,
        [FromServices] IRequestHandler<DeleteCitiesCommand.Request, OperationResponse> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();
}
