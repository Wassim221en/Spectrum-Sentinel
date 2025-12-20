using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.City.Commands.Delete;

public class DeleteCitiesHandler : IRequestHandler<DeleteCitiesCommand.Request, OperationResponse>
{
    private readonly IRepository _repository;

    public DeleteCitiesHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse> Handle(DeleteCitiesCommand.Request request, CancellationToken cancellationToken)
    {
        if (!request.CityIds.Any())
            return new HttpMessage("No city IDs provided", HttpStatusCode.BadRequest);

        var cities = await _repository.TrackingQuery<Domain.Entities.Settings.City>()
            .Where(c => request.CityIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        if (!cities.Any())
            return new HttpMessage("No cities found", HttpStatusCode.NotFound);

        _repository.SoftDeleteRange(cities);
        await _repository.SaveChangesAsync(cancellationToken);

        return OperationResponse.Ok();
    }
}

