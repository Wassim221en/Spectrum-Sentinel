using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Neighborhood.Queries.GetById;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Neighborhood.Commands.Modify;

public class ModifyNeighborhoodHandler : IRequestHandler<ModifyNeighborhoodCommand.Request, OperationResponse<GetNeighborhoodByIdQuery.Response>>
{
    private readonly IRepository _repository;

    public ModifyNeighborhoodHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetNeighborhoodByIdQuery.Response>> Handle(ModifyNeighborhoodCommand.Request request, CancellationToken cancellationToken)
    {
        var neighborhood = await _repository.TrackingQuery<Domain.Entities.Settings.Neighborhood>()
            .FirstOrDefaultAsync(n => n.Id == request.NeighborhoodId, cancellationToken);

        if (neighborhood is null)
            return new HttpMessage("Neighborhood not found", HttpStatusCode.NotFound);

        // Verify city exists
        var cityExists = await _repository.Query<Domain.Entities.Settings.City>()
            .AnyAsync(c => c.Id == request.CityId, cancellationToken);

        if (!cityExists)
            return new HttpMessage("City not found", HttpStatusCode.NotFound);

        neighborhood.Name = request.Name;
        neighborhood.CityId = request.CityId;

        _repository.Update(neighborhood);
        await _repository.SaveChangesAsync(cancellationToken);

        return await _repository.Query<Domain.Entities.Settings.Neighborhood>()
            .Include(n => n.City)
            .Where(n => n.Id == request.NeighborhoodId)
            .Select(GetNeighborhoodByIdQuery.Response.Selector())
            .FirstAsync(cancellationToken);
    }
}

