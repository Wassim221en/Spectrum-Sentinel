using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Neighborhood.Commands.Delete;

public class DeleteNeighborhoodsHandler : IRequestHandler<DeleteNeighborhoodsCommand.Request, OperationResponse>
{
    private readonly IRepository _repository;

    public DeleteNeighborhoodsHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse> Handle(DeleteNeighborhoodsCommand.Request request, CancellationToken cancellationToken)
    {
        if (!request.NeighborhoodIds.Any())
            return new HttpMessage("No neighborhood IDs provided", HttpStatusCode.BadRequest);

        var neighborhoods = await _repository.TrackingQuery<Domain.Entities.Settings.Neighborhood>()
            .Where(n => request.NeighborhoodIds.Contains(n.Id))
            .ToListAsync(cancellationToken);

        if (!neighborhoods.Any())
            return new HttpMessage("No neighborhoods found", HttpStatusCode.NotFound);

        _repository.SoftDeleteRange(neighborhoods);
        await _repository.SaveChangesAsync(cancellationToken);

        return OperationResponse.Ok();
    }
}

