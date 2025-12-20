using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Country.Commands.Delete;

public class DeleteCountriesHandler : IRequestHandler<DeleteCountriesCommand.Request, OperationResponse>
{
    private readonly IRepository _repository;

    public DeleteCountriesHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse> Handle(DeleteCountriesCommand.Request request, CancellationToken cancellationToken)
    {
        if (!request.CountryIds.Any())
            return new HttpMessage("No country IDs provided", HttpStatusCode.BadRequest);

        var countries = await _repository.TrackingQuery<Domain.Entities.Settings.Country>()
            .Where(c => request.CountryIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        if (!countries.Any())
            return new HttpMessage("No countries found", HttpStatusCode.NotFound);

        _repository.SoftDeleteRange(countries);
        await _repository.SaveChangesAsync(cancellationToken);

        return OperationResponse.Ok();
    }
}

