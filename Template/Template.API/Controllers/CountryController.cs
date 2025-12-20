using MediatR;
using Microsoft.AspNetCore.Mvc;
using Template.API;
using Template.API.Attributes;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Country.Commands.Add;
using Template.Dashboard.Country.Commands.Delete;
using Template.Dashboard.Country.Commands.Modify;
using Template.Dashboard.Country.Queries.GetAll;
using Template.Dashboard.Country.Queries.GetById;

[ApiController]
[Route("api/[controller]/[action]")]
[ApiGroup<SampleApiGroup>(SampleApiGroup.Dashboard)]
public class CountryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CountryController> _logger;

    public CountryController(IMediator mediator, ILogger<CountryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetAllCountriesQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllCountriesQuery.Request request,
        [FromServices] IRequestHandler<GetAllCountriesQuery.Request, OperationResponse<GetAllCountriesQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<GetCountryByIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromQuery] GetCountryByIdQuery.Request request,
        [FromServices] IRequestHandler<GetCountryByIdQuery.Request, OperationResponse<GetCountryByIdQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse<GetAllCountriesQuery.Response.CountryRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(
        [FromBody] AddCountryCommand.Request request,
        [FromServices] IRequestHandler<AddCountryCommand.Request, OperationResponse<GetAllCountriesQuery.Response.CountryRes>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpPut]
    [ProducesResponseType(typeof(OperationResponse<GetCountryByIdQuery.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Modify(
        [FromBody] ModifyCountryCommand.Request request,
        [FromServices] IRequestHandler<ModifyCountryCommand.Request, OperationResponse<GetCountryByIdQuery.Response>> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();


    [HttpDelete]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteCountriesCommand.Request request,
        [FromServices] IRequestHandler<DeleteCountriesCommand.Request, OperationResponse> handler)
        => (await handler.Handle(request, CancellationToken.None)).ToActionResult();
}
